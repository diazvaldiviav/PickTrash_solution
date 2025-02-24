using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using WebApplication1.Common.Exceptions;
using WebApplication1.Data.Repositories.Interfaces;
using WebApplication1.Geolocalization.Dtos;
using WebApplication1.Geolocalization.Services.Interfaces;

namespace WebApplication1.Geolocalization.Services.Implementations
{
    public class TrackingService : ITrackingService
    {
        private readonly IHubContext<LocationHub> _hubContext;
        private readonly IRequestRepository _requestRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TrackingService> _logger;
        private const int DEVIATION_THRESHOLD = 500; // metros
        private const int PROXIMITY_THRESHOLD = 1000; // metros

        public TrackingService(
            IHubContext<LocationHub> hubContext,
            IRequestRepository requestRepository,
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<TrackingService> logger)
        {
            _hubContext = hubContext;
            _requestRepository = requestRepository;
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<DirectionsResponseDTO> CalculateRouteAsync(double startLat, double startLng, double endLat, double endLng)
        {
            try
            {
                var apiKey = _configuration["GoogleMaps:ApiKey"];
                var url = $"https://maps.googleapis.com/maps/api/directions/json?" +
                         $"origin={startLat},{startLng}&" +
                         $"destination={endLat},{endLng}&" +
                         $"key={apiKey}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Google Directions API Response: {Content}", content); // Para debug

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<GoogleDirectionsResponse>(content, options);

                if (result?.Routes != null && result.Routes.Any() &&
                    result.Routes[0]?.Legs != null && result.Routes[0].Legs.Any())
                {
                    var route = result.Routes[0];
                    var leg = route.Legs[0];

                    return new DirectionsResponseDTO
                    {
                        EncodedPolyline = route.Overview_Polyline.Points,
                        DurationInSeconds = leg.Duration.Value,
                        DistanceInMeters = leg.Distance.Value
                    };
                }

                throw new Exception("No route found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating route from ({StartLat}, {StartLng}) to ({EndLat}, {EndLng})",
                    startLat, startLng, endLat, endLng);
                throw;
            }
        }

        public async Task UpdateDriverLocationAsync(int requestId, double latitude, double longitude)
        {
            try
            {
                var request = await _requestRepository.GetByIdAsync(requestId);
                if (request == null)
                {
                    _logger.LogWarning("Request {RequestId} not found", requestId);
                    return;
                }

                var distanceToPickup = CalculateDistance(
                    latitude,
                    longitude,
                    request.PickupLatitude,
                    request.PickupLongitude
                );

                var needsRecalculation = distanceToPickup > DEVIATION_THRESHOLD;
                DirectionsResponseDTO? newRoute = null;

                if (needsRecalculation)
                {
                    _logger.LogInformation("Recalculating route for request {RequestId} due to deviation", requestId);
                    newRoute = await CalculateRouteAsync(
                        latitude,
                        longitude,
                        request.PickupLatitude,
                        request.PickupLongitude
                    );
                }

                if (distanceToPickup <= PROXIMITY_THRESHOLD)
                {
                    _logger.LogInformation("Driver nearby for request {RequestId}. Distance: {Distance}m",
                        requestId, distanceToPickup);
                    await _hubContext.Clients.Group($"request_{requestId}")
                        .SendAsync("DriverNearby", distanceToPickup);
                }

                var updateDto = new TrackingUpdateDTO
                {
                    RequestId = requestId,
                    DriverLatitude = latitude,
                    DriverLongitude = longitude,
                    EstimatedMinutes = newRoute?.DurationInSeconds / 60 ?? 0,
                    DistanceToPickup = distanceToPickup,
                    RouteRecalculated = needsRecalculation,
                    EncodedPolyline = newRoute?.EncodedPolyline
                };

                await _hubContext.Clients.Group($"request_{requestId}")
                    .SendAsync("LocationUpdate", updateDto);

                _logger.LogInformation("Location updated for request {RequestId}: {Update}",
                    requestId, JsonSerializer.Serialize(updateDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating driver location for request {RequestId}", requestId);
                throw;
            }
        }

        public async Task StartTrackingAsync(int requestId, double driverLat, double driverLng)
        {
            try
            {
                var request = await _requestRepository.GetByIdAsync(requestId);
                if (request == null)
                {
                    throw new NotFoundException($"Request {requestId} not found");
                }

                var initialRoute = await CalculateRouteAsync(
                    driverLat,
                    driverLng,
                    request.PickupLatitude,
                    request.PickupLongitude
                );

                // Agregar log para ver la polyline antes de crear el DTO
                _logger.LogInformation("Initial route polyline: {Polyline}", initialRoute.EncodedPolyline);

                var updateDto = new TrackingUpdateDTO
                {
                    RequestId = requestId,
                    DriverLatitude = driverLat,
                    DriverLongitude = driverLng,
                    EstimatedMinutes = initialRoute.DurationInSeconds / 60,
                    DistanceToPickup = initialRoute.DistanceInMeters,
                    RouteRecalculated = false,
                    EncodedPolyline = initialRoute.EncodedPolyline
                };

                // Agregar log para ver el DTO completo
                _logger.LogInformation("Tracking update DTO: {@TrackingUpdate}", updateDto);

                await _hubContext.Clients.Group($"request_{requestId}")
                    .SendAsync("TrackingStarted", updateDto);

                _logger.LogInformation("Tracking started for request {RequestId}: {Update}",
                    requestId, JsonSerializer.Serialize(updateDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting tracking for request {RequestId}", requestId);
                throw;
            }
        }

        public async Task StopTrackingAsync(int requestId)
        {
            try
            {
                await _hubContext.Clients.Group($"request_{requestId}")
                    .SendAsync("TrackingStopped", requestId);

                _logger.LogInformation("Tracking stopped for request {RequestId}", requestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping tracking for request {RequestId}", requestId);
                throw;
            }
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371e3; // Radio de la tierra en metros
            var φ1 = lat1 * Math.PI / 180;
            var φ2 = lat2 * Math.PI / 180;
            var Δφ = (lat2 - lat1) * Math.PI / 180;
            var Δλ = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                    Math.Cos(φ1) * Math.Cos(φ2) *
                    Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using WebApplication1.Geolocalization.Dtos;
using WebApplication1.Geolocalization.Services.Interfaces;

namespace WebApplication1.Geolocalization
{
    public class LocationHub : Hub
    {
        private readonly ILogger<LocationHub> _logger;
        private readonly ITrackingService _trackingService;

        public LocationHub(ILogger<LocationHub> logger, ITrackingService trackingService)
        {
            _logger = logger;
            _trackingService = trackingService;
        }

        public async Task JoinRequestTracking(string requestId)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"request_{requestId}");
                _logger.LogInformation("Cliente unido al tracking de request: {RequestId}", requestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al unirse al tracking para request {RequestId}", requestId);
                throw;
            }
        }

        public async Task UpdateDriverLocation(LocationUpdateDTO location)
        {
            try
            {
                // Actualizar la ubicación y recalcular la ruta si es necesario
                await _trackingService.UpdateDriverLocationAsync(
                    location.RequestId,
                    location.Latitude,
                    location.Longitude
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating driver location");
                throw;
            }
        }


        public async Task LeaveRequestTracking(string requestId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"request_{requestId}");
        }
    }
}

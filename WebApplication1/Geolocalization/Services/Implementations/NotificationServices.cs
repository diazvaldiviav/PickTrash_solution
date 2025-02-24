using Microsoft.AspNetCore.SignalR;
using WebApplication1.Geolocalization.Services.Interfaces;
using WebApplication1.Models.Domain;

namespace WebApplication1.Geolocalization.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IHubContext<NotificationHub> hubContext,
            ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyRequestStatusChangeAsync(Request request)
        {
            try
            {
                var notification = new
                {
                    RequestId = request.Id,
                    NewStatus = request.Status,
                    Timestamp = DateTime.UtcNow
                };

                // Notificar al cliente
                await _hubContext.Clients
                    .Group($"user_{request.ClientId}")
                    .SendAsync("RequestStatusChanged", notification);

                _logger.LogInformation(
                    "Status change notification sent for request {RequestId}: {Status}",
                    request.Id,
                    request.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sending status change notification for request {RequestId}",
                    request.Id);
                throw;
            }
        }

        public async Task NotifyDriverNearbyAsync(int requestId, double distance)
        {
            try
            {
                var notification = new
                {
                    RequestId = requestId,
                    Distance = distance,
                    Message = $"Driver is {distance:N0} meters away",
                    Timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients
                    .Group($"request_{requestId}")
                    .SendAsync("DriverNearby", notification);

                _logger.LogInformation(
                    "Driver nearby notification sent for request {RequestId}. Distance: {Distance}m",
                    requestId,
                    distance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sending driver nearby notification for request {RequestId}",
                    requestId);
                throw;
            }
        }

        public async Task NotifyRouteRecalculatedAsync(int requestId, int newEta)
        {
            try
            {
                var notification = new
                {
                    RequestId = requestId,
                    NewEta = newEta,
                    Message = $"Route recalculated. New ETA: {newEta} minutes",
                    Timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients
                    .Group($"request_{requestId}")
                    .SendAsync("RouteRecalculated", notification);

                _logger.LogInformation(
                    "Route recalculation notification sent for request {RequestId}. New ETA: {NewEta}min",
                    requestId,
                    newEta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sending route recalculation notification for request {RequestId}",
                    requestId);
                throw;
            }
        }
    }
}

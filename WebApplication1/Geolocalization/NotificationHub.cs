using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace WebApplication1.Geolocalization
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation("User {UserId} joined notification group", userId);
        }

        public async Task JoinRequestGroup(string requestId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"request_{requestId}");
            _logger.LogInformation("Client joined request {RequestId} notification group", requestId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation(
                "Client disconnected from notification hub: {ConnectionId}",
                Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}

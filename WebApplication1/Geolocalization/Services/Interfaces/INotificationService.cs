using WebApplication1.Models.Domain;

namespace WebApplication1.Geolocalization.Services.Interfaces
{
    public interface INotificationService
    {
        Task NotifyRequestStatusChangeAsync(Request request);
        Task NotifyDriverNearbyAsync(int requestId, double distance);
        Task NotifyRouteRecalculatedAsync(int requestId, int newEta);
    }
}

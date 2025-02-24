using WebApplication1.Geolocalization.Dtos;

namespace WebApplication1.Geolocalization.Services.Interfaces
{
    public interface ITrackingService
    {
        Task StartTrackingAsync(int requestId, double driverLat, double driverLng);
        Task UpdateDriverLocationAsync(int requestId, double latitude, double longitude);
        Task<DirectionsResponseDTO> CalculateRouteAsync(double startLat, double startLng, double endLat, double endLng);
        Task StopTrackingAsync(int requestId);
    }
}

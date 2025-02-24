namespace WebApplication1.Geolocalization.Services.Interfaces
{
    public interface IGeocodingService
    {
        Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string address);
        Task<string?> GetAddressAsync(double latitude, double longitude);
    }
}

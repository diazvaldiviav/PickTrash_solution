namespace WebApplication1.Helpers
{
    public interface IGeocodingService
    {
        Task<(decimal Latitude, decimal Longitude)?> GetCoordinatesAsync(string address);
        Task<string?> GetAddressAsync(decimal latitude, decimal longitude);
    }
}

using WebApplication1.Models.Dtos.UserDto;

namespace WebApplication1.Services.Interfaces
{
    public interface IDriverService
    {
        Task<IEnumerable<AvailableDriverDTO>> GetActiveDriversAsync();
        Task<IEnumerable<AvailableDriverDTO>> GetDriversByLocationAsync(decimal latitude, decimal longitude, int radiusInKm);
        Task<UpdateLocationDTO?> GetLocationAsync(int driverId);
        Task UpdateLocationAsync(int driverId, UpdateLocationDTO location);
        Task UpdateLocationByAddressAsync(int driverId, string address);
    }

}

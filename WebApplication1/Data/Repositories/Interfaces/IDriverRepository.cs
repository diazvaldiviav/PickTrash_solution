using WebApplication1.Models.Domain;

namespace WebApplication1.Data.Repositories.Interfaces
{
    public interface IDriverRepository : IGenericRepository<Driver>
    {
        // Consultas básicas
        Task<Driver?> GetWithUserDetailsAsync(int driverId);
        Task<Driver?> GetWithVehiclesAsync(int driverId);

        // Consultas de estado y ubicación
        Task<IEnumerable<Driver>> GetActiveDriversAsync();
        Task<IEnumerable<Driver>> GetDriversByLocationAsync(decimal latitude, decimal longitude, int radiusInKm);
        Task<(decimal Latitude, decimal Longitude)?> GetLocationAsync(int driverId);
        Task UpdateLocationAsync(int driverId, decimal latitude, decimal longitude);
        Task UpdateActiveStatusAsync(int driverId, bool isActive);

        // Gestión de vehículos
        Task AddVehicleToDriverAsync(int driverId, int vehicleId);
        Task RemoveVehicleFromDriverAsync(int driverId, int vehicleId);
        Task<IEnumerable<Vehicle>> GetDriverVehiclesAsync(int driverId);

        // Autenticación
        Task<Driver> RegisterDriverAsync(Driver driver);
        Task UpdateDriverProfileAsync(int driverId, Driver updatedDriver);
        Task<bool> VerifyDriverDocumentsAsync(int driverId);
    }
}

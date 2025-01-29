using WebApplication1.Models.Domain;

namespace WebApplication1.Data.Repositories.Interfaces
{
    public interface IVehicleRepository: IGenericRepository<Vehicle>
    {
        Task<Vehicle> RegisterVehicleAsync(Vehicle vehicle);
        Task<Vehicle?> GetVehicleWithCategoryAsync(int id);
    }
}

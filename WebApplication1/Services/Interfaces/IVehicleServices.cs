using WebApplication1.Models.Dtos.Vehicle;

namespace WebApplication1.Services.Interfaces
{
    public interface IVehicleServices
    {
        Task<VehicleResponseDTO> RegisterVehicleAsync(CreateVehicleDTO dto, int id);
    }
}

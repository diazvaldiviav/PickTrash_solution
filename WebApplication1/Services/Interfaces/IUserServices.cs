using WebApplication1.Models.Dtos.UserDto;

namespace WebApplication1.Services.Interfaces
{
    public interface IUserServices
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();


        //cambiar a userService
        Task<IEnumerable<ClientDTO>> GetAllClientsAsync();
        Task<IEnumerable<DriverDTO>> GetAllDriversAsync();
    }
}

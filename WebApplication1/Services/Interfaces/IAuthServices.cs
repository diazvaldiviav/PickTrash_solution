using WebApplication1.Models.Domain;
using WebApplication1.Models.Dtos.UserDto;

namespace WebApplication1.Services.Interfaces
{
    public interface IAuthServices
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterUserDTO dto);
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);
        string GenerateJwtToken(User user);
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}

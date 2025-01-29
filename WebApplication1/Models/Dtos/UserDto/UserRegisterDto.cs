using WebApplication1.Models.Enums;

namespace WebApplication1.Models.Dtos.UserDto
{
    public class RegisterUserDTO
    {
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }

    // DTOs/Auth/LoginDTO.cs
    public class LoginDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }

    // DTOs/Auth/AuthResponseDTO.cs
    public class AuthResponseDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}

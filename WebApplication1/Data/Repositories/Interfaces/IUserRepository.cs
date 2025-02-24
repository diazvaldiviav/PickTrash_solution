using WebApplication1.Models.Domain;
using WebApplication1.Models.Enums;

namespace WebApplication1.Data.Repositories.Interfaces
{
    public interface IUserRepository: IGenericRepository<User>
    {
        // Consultas básicas
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username, UserRole role);
        Task<User?> GetWithRoleAsync(int userId);

        // Validaciones
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber);
        Task<bool> IsUsernameUniqueAsync(string username);

        // Consultas específicas por rol
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
        Task<bool> HasRoleAsync(int userId, UserRole role);


        //Authenticación
        Task<User> RegisterUserAsync(User user);
        Task<User?> AuthenticateAsync(string email, string passwordHash);
        Task UpdatePasswordAsync(int userId, string newPasswordHash);
        Task<bool> VerifyEmailAsync(int userId);
        Task<bool> ResetPasswordTokenAsync(string email, string token, DateTime expiryDate);


        //Validaciones de rol
        Task<bool> IsUsernameUniqueForRoleAsync(string username, UserRole role);
        Task<bool> IsPhoneNumberUniqueForRoleAsync(string phoneNumber, UserRole role);
        Task<bool> IsEmailUniqueForRoleAsync(string email, UserRole role);
    }
}

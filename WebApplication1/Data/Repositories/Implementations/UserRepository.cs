using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Repositories.Interfaces;
using WebApplication1.Models.Domain;
using WebApplication1.Models.Enums;
using WebApplication1.Common.Exceptions;

namespace WebApplication1.Data.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(PickTrashDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());
        }

        public async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber)
        {
            return !await _dbSet
                .AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            return !await _dbSet
                .AnyAsync(u => u.UserName.ToLower() == username.ToLower());
        }

        public async Task<bool> HasRoleAsync(int userId, UserRole role)
        {
            var user = await _dbSet.FindAsync(userId);
            if (user == null)
                return false;

            return user.Role == role;
        }

        public async Task<bool> VerifyEmailAsync(int userId)
        {
            var user = await _dbSet.FindAsync(userId);
            if (user == null)
                throw new NotFoundException($"Usuario con ID {userId} no encontrado");

            // Aquí podrías agregar lógica adicional para la verificación de email
            // Por ejemplo, actualizar un campo IsEmailVerified

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResetPasswordTokenAsync(string email, string token, DateTime expiryDate)
        {
            var user = await GetByEmailAsync(email);
            if (user == null)
                throw new NotFoundException($"Usuario con email {email} no encontrado");

            // Aquí deberías almacenar el token y su fecha de expiración
            // Podrías necesitar agregar estos campos a tu modelo de User:
            // - PasswordResetToken
            // - PasswordResetTokenExpiry

            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = expiryDate;

            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<User?> GetWithRoleAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.Client)
                .Include(u => u.Driver)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _dbSet.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _dbSet
                .Where(u => u.Role == role)
                .ToListAsync();
        }

        //authenticacion
        public async Task<User> RegisterUserAsync(User user)
        {
            // Validar unicidad de email y teléfono
            if (!await IsEmailUniqueAsync(user.Email))
                throw new BadRequestException("El email ya está registrado");

            if (!await IsPhoneNumberUniqueAsync(user.PhoneNumber))
                throw new BadRequestException("El número de teléfono ya está registrado");

            // Registrar usuario
            await _dbSet.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> AuthenticateAsync(string email, string passwordHash)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == email.ToLower() &&
                    u.PasswordHash == passwordHash);
        }

        public async Task UpdatePasswordAsync(int userId, string newPasswordHash)
        {
            var user = await _dbSet.FindAsync(userId);
            if (user == null)
                throw new NotFoundException("Usuario no encontrado");

            user.PasswordHash = newPasswordHash;
            await _context.SaveChangesAsync();
        }
    }
}

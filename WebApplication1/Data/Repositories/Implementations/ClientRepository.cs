using Microsoft.EntityFrameworkCore;
using WebApplication1.Common.Exceptions;
using WebApplication1.Data.Repositories.Interfaces;
using WebApplication1.Models.Domain;

namespace WebApplication1.Data.Repositories.Implementations
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        public ClientRepository(PickTrashDbContext context) : base(context)
        {
        }

        public async Task<Client?> GetWithUserDetailsAsync(int clientId)
        {
            return await _dbSet
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == clientId);
        }

        public async Task<Client?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _dbSet
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User.PhoneNumber == phoneNumber);
        }

        public async Task<Client?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<IEnumerable<Client>> GetAllWithUserDetailsAsync()
        {
            return await _dbSet
                .Include(c => c.User)
                .ToListAsync();
        }


        //register client

        public async Task<Client> RegisterClientAsync(Client client)
        {
            await _dbSet.AddAsync(client);
            await _context.SaveChangesAsync();

            // Retornar cliente con datos de usuario incluidos
            return await GetWithUserDetailsAsync(client.Id);
        }

        public async Task UpdateClientProfileAsync(int clientId, Client updatedClient)
        {
            var existingClient = await _dbSet.FindAsync(clientId);
            if (existingClient == null)
                throw new NotFoundException("Cliente no encontrado");

            existingClient.DefaultAddress = updatedClient.DefaultAddress;
            // Actualizar otras propiedades específicas del cliente

            await _context.SaveChangesAsync();
        }
    }
}

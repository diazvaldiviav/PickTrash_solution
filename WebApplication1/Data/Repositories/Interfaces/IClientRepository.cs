using WebApplication1.Models.Domain;

namespace WebApplication1.Data.Repositories.Interfaces
{
    public interface IClientRepository: IGenericRepository<Client>
    {
        Task<Client?> GetByUserIdAsync(int userId);
        Task<Client?> GetWithUserDetailsAsync(int clientId);
        Task<Client?> GetByPhoneNumberAsync(string phoneNumber);
        Task<IEnumerable<Client>> GetAllWithUserDetailsAsync();
        Task<Client> RegisterClientAsync(Client client);
        Task UpdateClientProfileAsync(int clientId, Client updatedClient);
    }
    
}

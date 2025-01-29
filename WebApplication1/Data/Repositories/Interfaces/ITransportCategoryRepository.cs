using WebApplication1.Models.Domain;

namespace WebApplication1.Data.Repositories.Interfaces
{
    public interface ITransportCategoryRepository: IGenericRepository<TransportCategory>
    {
        Task<TransportCategory?> GetByCategoryNameAsync(string categoryName);
        Task<TransportCategory?> GetByWeightRangeAsync(decimal weight);
    }
}

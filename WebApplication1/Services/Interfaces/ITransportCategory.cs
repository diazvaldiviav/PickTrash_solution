using WebApplication1.Models.Dtos.TransportCategory;

namespace WebApplication1.Services.Interfaces
{
    public interface ITransportCategory
    {
        Task<IEnumerable<TransportCategoryDTO>> GetAllCategoriesAsync();
    }
}

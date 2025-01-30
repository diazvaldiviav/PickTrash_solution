using AutoMapper;
using WebApplication1.Data.Repositories.Interfaces;
using WebApplication1.Models.Dtos.TransportCategory;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Implementations
{
    public class TransportCategoryService : ITransportCategory
    {
        private readonly ITransportCategoryRepository _transportCategoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TransportCategoryService> _logger;

        public TransportCategoryService(
            ITransportCategoryRepository transportCategoryRepository,
            IMapper mapper,
            ILogger<TransportCategoryService> logger)
        {
            _transportCategoryRepository = transportCategoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TransportCategoryDTO>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _transportCategoryRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<TransportCategoryDTO>>(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las categorías de transporte");
                throw;
            }
        }
    }
}

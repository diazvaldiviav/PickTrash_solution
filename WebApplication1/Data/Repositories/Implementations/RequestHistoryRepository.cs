using Microsoft.EntityFrameworkCore;
using WebApplication1.Common.Exceptions;
using WebApplication1.Models.Domain;
using WebApplication1.Data.Repositories.Interfaces;

namespace WebApplication1.Data.Repositories.Implementations
{
    public class RequestHistoryRepository: GenericRepository<RequestHistory>, IRequestHistoryRepository
    {
        private readonly ILogger<RequestRepository> _logger;
        public RequestHistoryRepository(PickTrashDbContext context, ILogger<RequestRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public override async Task<RequestHistory> AddAsync(RequestHistory requestHistory)
        {
            await _dbSet.AddAsync(requestHistory);

            // Cargar la relación con Request y sus relaciones asociadas
            var entry = _context.Entry(requestHistory);
            await entry.Reference(rh => rh.Request).Query()
                .Include(r => r.Client)
                    .ThenInclude(c => c.User)
                .Include(r => r.Driver)
                    .ThenInclude(d => d.User)
                .LoadAsync();

            return requestHistory;
        }

        public async Task UpdateAsync(Vehicle vehicle)
        {
            var existingVehicle = await GetByIdAsync(vehicle.Id);
            if (existingVehicle == null)
                throw new NotFoundException($"Vehículo con ID {vehicle.Id} no encontrado");

            _context.Entry(existingVehicle).CurrentValues.SetValues(vehicle);
            await _context.SaveChangesAsync();
        }


        public override async Task DeleteAsync(int id)
        {
            var request = await _context.RequestHistories
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                throw new NotFoundException($"Vehículo con ID {id} no encontrado");

            _context.Remove(request);
            await _context.SaveChangesAsync();
        }

        public override async Task<RequestHistory> GetByIdAsync(int id)
        {
            var requestHistory = await _dbSet.FirstOrDefaultAsync(rh => rh.Id == id);

            if (requestHistory == null) throw new NotFoundException($"RequestHistory con ID {id} no encontrado");

            return requestHistory;
        }
    }
}

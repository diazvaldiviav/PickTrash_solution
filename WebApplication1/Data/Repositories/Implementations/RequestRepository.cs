using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Repositories.Interfaces;
using WebApplication1.Models.Domain;
using WebApplication1.Models.Enums;

namespace WebApplication1.Data.Repositories.Implementations
{
    public class RequestRepository : GenericRepository<Request>, IRequestRepository
    {
        private readonly ILogger<RequestRepository> _logger;
        public RequestRepository(PickTrashDbContext context, ILogger<RequestRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public override async Task<Request?> GetByIdAsync(int id)
        {
            var request = await _dbSet.FirstOrDefaultAsync(r => r.Id == id);

            return request;
        }

        public async Task<IEnumerable<Request>> GetRequestsByClientIdAsync(int clientId)
        {
            return await _dbSet
                .Where(r => r.ClientId == clientId)
                .Include(r => r.Driver)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetRequestsByDriverIdAsync(int driverId)
        {
            var requests = await _dbSet
                 .Where(r => r.DriverId == driverId)
                 .OrderByDescending(r => r.CreatedAt)
                 .ToListAsync();

            // Logging mejorado
            _logger.LogInformation("Buscando requests para driverId: {DriverId}", driverId);
            _logger.LogInformation("Total de requests encontrados: {Count}", requests.Count);

            foreach (var request in requests)
            {
                _logger.LogInformation(
                    "Request ID: {RequestId}, ClientId: {ClientId}, Status: {Status}, ScheduledDate: {Date}",
                    request.Id,
                    request.ClientId,
                    request.Status,
                    request.ScheduledDate
                );
            }

            // Debug: Verificar si hay requests en la base de datos
            var totalRequests = await _dbSet.CountAsync();
            _logger.LogInformation("Total de requests en la base de datos: {Total}", totalRequests);

            // Debug: Verificar algunos requests con ese driverId
            var requestsWithDriver = await _dbSet
                .Where(r => r.DriverId == driverId)
                .Select(r => new { r.Id, r.DriverId })
                .ToListAsync();

            _logger.LogInformation("Requests encontrados con DriverId {DriverId}: {Count}",
                driverId,
                requestsWithDriver.Count);

            return requests;

        }

        public async Task<IEnumerable<Request>> GetRequestsByStatusAsync(int driverId, RequestStatus status)
        {
           
            return await _dbSet
            .Where(r => r.DriverId == driverId && r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetPendingRequestsInAreaAsync(
            double latitude,
            double longitude,
            double radiusInKm)
        {
            // Aquí implementarías la lógica de búsqueda por proximidad
            // Podrías usar una función de base de datos específica para cálculos geoespaciales
            return await _dbSet
                .Where(r => r.Status == RequestStatus.Pending)
                .ToListAsync();
        }

        public async Task<bool> HasOverlappingRequestsAsync(
            int driverId,
            DateTime scheduledDate,
            TimeSpan duration)
        {
            var endTime = scheduledDate.Add(duration);

            return await _dbSet.AnyAsync(r =>
                r.DriverId == driverId &&
                r.Status != RequestStatus.Completed &&
                r.Status != RequestStatus.Cancelled &&
                r.ScheduledDate < endTime &&
                r.ScheduledDate.Add(duration) > scheduledDate);
        }

        public async Task<bool> CanModifyRequestAsync(int requestId)
        {
            var request = await GetByIdAsync(requestId);
            if (request == null) return false;

            // Verifica si faltan más de 48 horas para el servicio
            return request.ScheduledDate.Subtract(DateTime.UtcNow).TotalHours > 48;
        }

        public async Task UpdateStatusAsync(int requestId, RequestStatus newStatus, string changedBy, string? reason = null)
        {
            var request = await GetByIdAsync(requestId);
            if (request == null) return;

            var oldStatus = request.Status;
            request.Status = newStatus;
            request.LastModified = DateTime.UtcNow;
            request.LastModifiedBy = changedBy;

            // Crear historial del cambio
            var history = new RequestHistory
            {
                RequestId = requestId,
                PreviousStatus = oldStatus,
                NewStatus = newStatus,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = changedBy,
                ChangeReason = reason // Agregamos la razón al historial
            };

            await _context.RequestHistories.AddAsync(history);
        }


        public override async Task<Request> AddAsync(Request request)
        {
            await _dbSet.AddAsync(request);

            // Cargar las relaciones necesarias
            var entry = _context.Entry(request);
            await entry.Reference(r => r.Driver).Query()
                .Include(d => d.User)
                .LoadAsync();
            await entry.Reference(r => r.Client).Query()
                .Include(c => c.User)
                .LoadAsync();

            return request;
        }
    }
}

using AutoMapper;
using WebApplication1.Common.Exceptions;
using WebApplication1.Data.Repositories.Interfaces;
using WebApplication1.Models.Domain;
using WebApplication1.Models.Dtos.Request;
using WebApplication1.Models.Enums;
using WebApplication1.Services.Interfaces;
using WebApplication1.Helpers;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Repositories.Implementations;
using WebApplication1.Geolocalization.Services.Interfaces;
using WebApplication1.Geolocalization.Services.Implementations;


namespace WebApplication1.Services.Implementations
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RequestService> _logger;
        private readonly IDriverRepository _driverRepository;
        private readonly IGeocodingService _geocodingService;
        private readonly IClientRepository _clientRepository;
        private readonly ITrackingService _trackingService;
        private readonly INotificationService _notificationService;
        private readonly IRequestHistoryRepository _requestHistoryRepository;

        public RequestService(
            IRequestRepository requestRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<RequestService> logger,
            IDriverRepository driverRepository,
            IGeocodingService geocodingService,
            IClientRepository clientRepository,
            ITrackingService trackingService,
            INotificationService notificationService,
            IRequestHistoryRepository requestHistoryRepository
            )
        {
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _driverRepository = driverRepository;
            _geocodingService = geocodingService;
            _clientRepository = clientRepository;
            _trackingService = trackingService;
            _notificationService = notificationService;
            _requestHistoryRepository = requestHistoryRepository;
        }

        public async Task<RequestDTO> GetRequestByIdAsync(int id)
        {
            var request = await _requestRepository.GetByIdAsync(id);
            if (request == null)
                throw new NotFoundException($"Request with ID {id} not found");

            return _mapper.Map<RequestDTO>(request);
        }

        public async Task<IEnumerable<RequestDTO>> GetAllRequestsAsync()
        {
            var requests = await _requestRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RequestDTO>>(requests);
        }

        public async Task<RequestDTO> CreateRequestAsync(CreateRequestDTO createDto, int clientId)
        {
            try
            {
                // Validar que el conductor exista y esté disponible
                var driver = await _driverRepository.GetByIdAsync(createDto.DriverId);
                if (driver == null)
                    throw new NotFoundException($"Conductor con ID {createDto.DriverId} no encontrado");

                if (!driver.IsAvailable)
                    throw new BadRequestException("El conductor seleccionado no está disponible");

                // Convertir direcciones a coordenadas
                var pickupCoordinates = await _geocodingService.GetCoordinatesAsync(createDto.PickupAddress);
                if (!pickupCoordinates.HasValue)
                    throw new BadRequestException("No se pudo obtener las coordenadas de la dirección de recogida");

                var dropoffCoordinates = await _geocodingService.GetCoordinatesAsync(createDto.DropoffAddress);
                if (!dropoffCoordinates.HasValue)
                    throw new BadRequestException("No se pudo obtener las coordenadas de la dirección de entrega");


                // Validar que el cliente exista
                var client = await _clientRepository.GetByIdAsync(clientId);
               

                if (client == null)
                    throw new NotFoundException($"Cliente con ID {clientId} no encontrado");


                var request = _mapper.Map<Request>(createDto);
                request.ClientId = clientId;
                request.ClientName = client.User.Name;  
                request.DriverName = driver.User.Name;
                request.Status = RequestStatus.Pending;
                request.CreatedAt = DateTime.UtcNow;
                request.PickupLatitude = pickupCoordinates.Value.Latitude;
                request.PickupLongitude = pickupCoordinates.Value.Longitude;
                request.LastModifiedAt = DateTime.UtcNow;
                request.LastModifiedBy = clientId.ToString();

                var createdRequest = await _requestRepository.AddAsync(request);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Request created successfully. RequestId: {RequestId}", createdRequest.Id);

                return _mapper.Map<RequestDTO>(createdRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating request for client {ClientId}", clientId);
                throw;
            }
        }

        public async Task<RequestDTO> UpdateRequestAsync(int id, UpdateRequestDTO updateDto)
        {
            var request = await _requestRepository.GetByIdAsync(id);
            if (request == null)
                throw new NotFoundException($"Request with ID {id} not found");

            if (!await CanModifyRequestAsync(id))
                throw new BadRequestException("Request can't be modified within 48 hours of scheduled date");

            _mapper.Map(updateDto, request);
            await _requestRepository.UpdateAsync(request);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RequestDTO>(request);
        }

        public async Task<IEnumerable<RequestDTO>> GetClientRequestsAsync(int clientId)
        {
            var requests = await _requestRepository.GetRequestsByClientIdAsync(clientId);
            return _mapper.Map<IEnumerable<RequestDTO>>(requests);
        }

        public async Task<IEnumerable<RequestDTO>> GetDriverRequestsAsync(int driverId)
        {
            var requests = await _requestRepository.GetRequestsByDriverIdAsync(driverId);
            return _mapper.Map<IEnumerable<RequestDTO>>(requests);
        }

        public async Task<RequestDTO> AcceptRequestAsync(int requestId, int driverId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new NotFoundException($"Request with ID {requestId} not found");

            if (request.Status != RequestStatus.Pending)
                throw new BadRequestException("Request is not in pending status");

            if (!await CanDriverAcceptRequestAsync(driverId, requestId))
                throw new BadRequestException("Driver cannot accept this request due to schedule overlap");

            request.DriverId = driverId;
            request.Status = RequestStatus.Accepted;

            await _requestRepository.UpdateAsync(request);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RequestDTO>(request);
        }

        public async Task<bool> CanDriverAcceptRequestAsync(int driverId, int requestId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null) return false;

            return true;


          //  return !await _requestRepository.HasOverlappingRequestsAsync(
             //   driverId,
              //  request.ScheduledDate,
               // TimeSpan.FromHours(2)); // Duración estimada del servicio
        }

        public async Task<bool> CanModifyRequestAsync(int requestId)
        {
            return await _requestRepository.CanModifyRequestAsync(requestId);
        }

        public async Task<IEnumerable<RequestDTO>> GetAvailableRequestsForDriverAsync(
       int driverId,
       double latitude,
       double longitude)
        {
            try
            {
                var availableRequests = await _requestRepository.GetPendingRequestsInAreaAsync(
                    latitude,
                    longitude,
                    radiusInKm: 20); // Radio configurable

                var requests = availableRequests.ToList();
                var filteredRequests = new List<Request>();


                foreach (var request in requests)
                {
                    if (await CanDriverAcceptRequestAsync(driverId, request.Id))
                    {
                        filteredRequests.Add(request);
                    }
                }

                return _mapper.Map<IEnumerable<RequestDTO>>(filteredRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available requests for driver {DriverId}", driverId);
                throw;
            }
        }

        public async Task<RequestDTO> StartServiceAsync(int requestId, double driverLat, double driverLng)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new NotFoundException($"Request {requestId} not found");

            // Validar estado actual
            if (request.Status != RequestStatus.Accepted)
                throw new BadRequestException("Request must be in CONFIRMED status to start service");

            try
            {
                var oldStatus = request.Status;
                request.Status = RequestStatus.InProgress;
                request.LastModifiedAt = DateTime.UtcNow;

                // Crear historial
                var history = new RequestHistory
                {
                    RequestId = requestId,
                    OldStatus = oldStatus,
                    NewStatus = RequestStatus.InProgress,
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = request.DriverName,
                    ChangeReason = "Driver started the service"
                };

                await _requestHistoryRepository.AddAsync(history);

                // Iniciar el tracking con la ubicación proporcionada
                await _trackingService.StartTrackingAsync(
                    requestId,
                    driverLat,    // Usando la ubicación recibida
                    driverLng     // del conductor
                );

                await _unitOfWork.SaveChangesAsync();

                // Notificar al cliente
                await _notificationService.NotifyRequestStatusChangeAsync(request);

                return _mapper.Map<RequestDTO>(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting service for request {RequestId}", requestId);
                throw;
            }
        }

        public async Task<RequestDTO> CompleteServiceAsync(int requestId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new NotFoundException($"Request with ID {requestId} not found");

            if (request.Status != RequestStatus.InProgress)
                throw new BadRequestException("Request must be in InProgress status to complete service");

            request.Status = RequestStatus.Completed;
            request.CompletedAt = DateTime.UtcNow;

            await _requestRepository.UpdateStatusAsync(requestId, RequestStatus.Completed, "Driver");
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Service completed for request {RequestId}", requestId);
            return _mapper.Map<RequestDTO>(request);
        }


        public async Task<RequestDTO> CancelRequestAsync(int requestId, string reason)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new NotFoundException($"Request with ID {requestId} not found");

            if (request.Status == RequestStatus.Completed || request.Status == RequestStatus.Cancelled)
                throw new BadRequestException("Cannot cancel a completed or already cancelled request");

            if (!await CanModifyRequestAsync(requestId))
                throw new BadRequestException("Cannot cancel request within 48 hours of scheduled date");

            request.Status = RequestStatus.Cancelled;

            // Crear historial con razón de cancelación
            await _requestRepository.UpdateStatusAsync(
                requestId,
                RequestStatus.Cancelled,
                "Client",
                reason);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Request {RequestId} cancelled. Reason: {Reason}", requestId, reason);
            return _mapper.Map<RequestDTO>(request);
        }


        public async Task<RequestDTO> UpdatePriceAsync(int requestId, decimal newPrice, string updatedBy)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new NotFoundException($"Request with ID {requestId} not found");

            if (request.Status != RequestStatus.Pending && request.Status != RequestStatus.Accepted)
                throw new BadRequestException("Price can only be updated for pending or accepted requests");

            var oldPrice = request.ProposedPrice;
            request.ProposedPrice = newPrice;

            // Registrar historial de cambio de precio
            var history = new RequestHistory
            {
                RequestId = requestId,
                PreviousStatus = request.Status,
                NewStatus = request.Status,
                ChangedBy = updatedBy,
                PreviousPrice = oldPrice,
                NewPrice = newPrice,
                ChangeReason = "Price negotiation"
            };

            await _requestRepository.UpdateAsync(request);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Price updated for request {RequestId}. Old price: {OldPrice}, New price: {NewPrice}",
                requestId, oldPrice, newPrice);

            return _mapper.Map<RequestDTO>(request);
        }


        public async Task<IEnumerable<RequestDTO>> GetRequestsByDateRangeAsync(
       int userId,
       DateTime startDate,
       DateTime endDate,
       string userRole)
        {
            var requests = userRole.ToLower() == "driver"
                ? await _requestRepository.GetRequestsByDriverIdAsync(userId)
                : await _requestRepository.GetRequestsByClientIdAsync(userId);

            var filteredRequests = requests.Where(r =>
                r.ScheduledDate.Date >= startDate.Date &&
                r.ScheduledDate.Date <= endDate.Date);

            return _mapper.Map<IEnumerable<RequestDTO>>(filteredRequests);
        }


        public async Task<IEnumerable<RequestDTO>> GetDriverRequestsAsync(int driverId, RequestStatus? status = null)
        {
            try
            {
                IEnumerable<Request> requests;

                if (status.HasValue)
                {
                    requests = await _requestRepository.GetRequestsByStatusAsync(driverId, status.Value);
                }
                else
                {
                    requests = await _requestRepository.GetRequestsByDriverIdAsync(driverId);
                }

                return _mapper.Map<IEnumerable<RequestDTO>>(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting requests for driver {DriverId}", driverId);
                throw;
            }
        }

    }
}

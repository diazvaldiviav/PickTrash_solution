using AutoMapper;
using WebApplication1.Common.Exceptions;
using WebApplication1.Data.Repositories.Interfaces;
using WebApplication1.Models.Domain;
using WebApplication1.Models.Dtos.Vehicle;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Implementations
{
    public class VehicleService: IVehicleServices
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ITransportCategoryRepository _transportCategoryRepository;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(
            IVehicleRepository vehicleRepository,
            IDriverRepository driverRepository,
            IMapper mapper,
           ILogger<VehicleService> logger,
          IUserRepository userRepository,
          ITransportCategoryRepository transportCategoryRepository
          )
        {
            _vehicleRepository = vehicleRepository;
            _driverRepository = driverRepository;
            _mapper = mapper;
            _logger = logger;
            _userRepository = userRepository;
            _transportCategoryRepository = transportCategoryRepository;
        }


        public async Task<VehicleResponseDTO> RegisterVehicleAsync(CreateVehicleDTO dto, int driverId)
        {
            try
            {
                // Crear el vehículo
                var vehicle = _mapper.Map<Vehicle>(dto);

             


                // Asignar la categoría de transporte, si existe
               var Category = await _transportCategoryRepository.GetByCategoryNameAsync(dto.TransportCategoryName);
                if (Category == null)
                    throw new BadRequestException("No se encontró una categoría válida para el peso especificado");



                vehicle.TransportCategory = Category;

                // Registrar el vehículo
                var registeredVehicle = await _vehicleRepository.RegisterVehicleAsync(vehicle);

                //obtener el conductor
                var driver = await _driverRepository.GetByIdAsync(driverId);

                //obtener el usuario

                 var user = await _userRepository.GetByIdAsync(driverId);

                // Asignar el vehículo al conductor
                await _driverRepository.AddVehicleToDriverAsync(driverId, registeredVehicle.Id);

                _logger.LogInformation(
                    "Se registro el vehiculo a nombre de {Name} con id {DriverId} con placa {Plate}",
                    user?.Name,
                    driverId,
                    registeredVehicle.Plate
                    );

                return _mapper.Map<VehicleResponseDTO>(registeredVehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el vehículo con placa {Plate} para el conductor {DriverId}",
                    dto.Plate, driverId);
                throw;
            }
        }
    }
}

using AutoMapper;
using WebApplication1.Data.Repositories.Interfaces;
using WebApplication1.Models.Dtos.UserDto;
using WebApplication1.Services.Interfaces;
using WebApplication1.Helpers;
using WebApplication1.Common.Exceptions;
using WebApplication1.Geolocalization.Services.Interfaces;

namespace WebApplication1.Services.Implementations
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _driverRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DriverService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGeocodingService _geocodingService;

        public DriverService(
            IDriverRepository driverRepository,
            IMapper mapper,
            ILogger<DriverService> logger,
            IUnitOfWork unitOfWork,
            IGeocodingService geocodingService
            )
        {
            _driverRepository = driverRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _geocodingService = geocodingService;
        }

        public async Task<IEnumerable<AvailableDriverDTO>> GetActiveDriversAsync()
        {
            try
            {
                var drivers = await _driverRepository.GetActiveDriversAsync();
                return _mapper.Map<IEnumerable<AvailableDriverDTO>>(drivers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active drivers");
                throw;
            }
        }

        public async Task<IEnumerable<AvailableDriverDTO>> GetDriversByLocationAsync(
            decimal latitude,
            decimal longitude,
            int radiusInKm)
        {
            try
            {
                var drivers = await _driverRepository.GetDriversByLocationAsync(
                    latitude,
                    longitude,
                    radiusInKm);
                return _mapper.Map<IEnumerable<AvailableDriverDTO>>(drivers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting drivers by location");
                throw;
            }
        }


        public async Task UpdateLocationAsync(int driverId, UpdateLocationDTO location)
        {
            try
            {
                await _driverRepository.UpdateLocationAsync(
                    driverId,
                    location.Latitude,
                    location.Longitude);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating driver location for driver {DriverId}", driverId);
                throw;
            }
        }

        public async Task<UpdateLocationDTO?> GetLocationAsync(int driverId)
        {
            try
            {
                var location = await _driverRepository.GetLocationAsync(driverId);


                if (!location.HasValue) return null;

                return new UpdateLocationDTO
                {
                    Latitude = location.Value.Latitude,
                    Longitude = location.Value.Longitude
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting driver location for driver {DriverId}", driverId);
                throw;
            }
        }

        public async Task UpdateLocationByAddressAsync(int driverId, string address)
        {
            var coordinates = await _geocodingService.GetCoordinatesAsync(address);

            if (!coordinates.HasValue)
                throw new BadRequestException("No se pudo convertir la dirección a coordenadas");

            await _driverRepository.UpdateLocationAsync(
                driverId,
                coordinates.Value.Latitude,
                coordinates.Value.Longitude);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}

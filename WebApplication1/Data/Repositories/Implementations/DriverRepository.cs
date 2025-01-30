using Microsoft.EntityFrameworkCore;
using WebApplication1.Common.Exceptions;
using WebApplication1.Data.Repositories.Interfaces;
using WebApplication1.Models.Domain;

namespace WebApplication1.Data.Repositories.Implementations
{
    public class DriverRepository : GenericRepository<Driver>, IDriverRepository
    {
        public DriverRepository(PickTrashDbContext context) : base(context)
        {
        }


        public async Task<Driver?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<Driver?> GetWithUserDetailsAsync(int driverId)
        {
            return await _dbSet
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == driverId);
        }

        public async Task<Driver?> GetWithVehiclesAsync(int driverId)
        {
            return await _dbSet
                .Include(d => d.User)
                .Include(d => d.DriverVehicles)
                    .ThenInclude(dv => dv.Vehicle)
                        .ThenInclude(v => v.TransportCategory)
                .FirstOrDefaultAsync(d => d.Id == driverId);
        }

        public async Task<IEnumerable<Driver>> GetActiveDriversAsync()
        {
            return await _dbSet
                .Include(d => d.User)
                .Include(d => d.DriverVehicles)
                    .ThenInclude(dv => dv.Vehicle)
                .Where(d => d.IsAvailable)
                .ToListAsync();
        }

        public async Task<IEnumerable<Driver>> GetDriversByLocationAsync(
            decimal latitude,
            decimal longitude,
            int radiusInKm)
        {
            // Usando la fórmula de Haversine para calcular la distancia
            return await _dbSet
                .Include(d => d.User)
                .Include(d => d.DriverVehicles)
                    .ThenInclude(dv => dv.Vehicle)
                .Where(d => d.IsAvailable)
                .Where(d =>
                    6371 * Math.Acos(
                        Math.Cos(Convert.ToDouble(latitude) * (Math.PI / 180)) *
                        Math.Cos(Convert.ToDouble(d.Latitude) * (Math.PI / 180)) *
                        Math.Cos((Convert.ToDouble(d.Longitude) - Convert.ToDouble(longitude)) * (Math.PI / 180)) +
                        Math.Sin(Convert.ToDouble(latitude) * (Math.PI / 180)) *
                        Math.Sin(Convert.ToDouble(d.Latitude) * (Math.PI / 180))
                    ) <= radiusInKm)
                .ToListAsync();
        }

        public async Task UpdateLocationAsync(int driverId, decimal latitude, decimal longitude)
        {
            var driver = await _dbSet.FindAsync(driverId);
            if (driver == null)
                throw new NotFoundException($"Conductor con ID {driverId} no encontrado");

            driver.Latitude = latitude;
            driver.Longitude = longitude;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateActiveStatusAsync(int driverId, bool isActive)
        {
            var driver = await _dbSet.FindAsync(driverId);
            if (driver == null)
                throw new NotFoundException($"Conductor con ID {driverId} no encontrado");

            driver.IsAvailable = isActive;
            await _context.SaveChangesAsync();
        }


        public async Task AddVehicleToDriverAsync(int driverId, int vehicleId)
        {

            //get driver
            var driver = await _dbSet
                .Include(d => d.DriverVehicles)
                .FirstOrDefaultAsync(d => d.Id == driverId);

            if (driver == null)
                throw new NotFoundException($"Conductor con ID {driverId} no encontrado");

            // Verificar si el vehículo ya está asignado al conductor
            if (driver.DriverVehicles.Any(dv => dv.VehicleId == vehicleId))
                throw new ConflictException("El vehículo ya está asignado a este conductor");


            // Crear la relación entre conductor y vehículo
            var driverVehicle = new DriverVehicle
            {
                DriverId = driverId,
                VehicleId = vehicleId
            };

            await _context.DriverVehicles.AddAsync(driverVehicle);
            await _context.SaveChangesAsync();
        }


        public async Task RemoveVehicleFromDriverAsync(int driverId, int vehicleId)
        {
            var driverVehicle = await _context.DriverVehicles
                .FirstOrDefaultAsync(dv =>
                    dv.DriverId == driverId &&
                    dv.VehicleId == vehicleId);

            if (driverVehicle == null)
                throw new NotFoundException(
                    $"No se encontró la relación entre el conductor {driverId} y el vehículo {vehicleId}");

            _context.DriverVehicles.Remove(driverVehicle);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetDriverVehiclesAsync(int driverId)
        {
            var driver = await _dbSet
                .Include(d => d.DriverVehicles)
                    .ThenInclude(dv => dv.Vehicle)
                        .ThenInclude(v => v.TransportCategory)
                .FirstOrDefaultAsync(d => d.Id == driverId);

            if (driver == null)
                throw new NotFoundException($"Conductor con ID {driverId} no encontrado");

            return driver.DriverVehicles
                .Select(dv => dv.Vehicle)
                .ToList();
        }




        //Authenticating driver

        public async Task<Driver> RegisterDriverAsync(Driver driver)
        {
            // Por defecto, un conductor nuevo está inactivo hasta verificar documentos pero sera para mas tarde
           // driver.IsAvailable = false;

            await _dbSet.AddAsync(driver);
            await _context.SaveChangesAsync();

            // Retornar conductor con datos de usuario incluidos
            return await GetWithUserDetailsAsync(driver.Id);
        }

        public async Task UpdateDriverProfileAsync(int driverId, Driver updatedDriver)
        {
            var existingDriver = await _dbSet.FindAsync(driverId);
            if (existingDriver == null)
                throw new NotFoundException("Conductor no encontrado");

            existingDriver.Latitude = updatedDriver.Latitude;
            existingDriver.Longitude = updatedDriver.Longitude;
            // Actualizar otras propiedades específicas del conductor

            await _context.SaveChangesAsync();
        }

        public async Task<bool> VerifyDriverDocumentsAsync(int driverId)
        {
            var driver = await _dbSet.FindAsync(driverId);
            if (driver == null)
                throw new NotFoundException("Conductor no encontrado");

            driver.IsAvailable = true;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

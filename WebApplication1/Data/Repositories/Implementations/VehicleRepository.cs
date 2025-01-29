using Microsoft.EntityFrameworkCore;
using WebApplication1.Common.Exceptions;
using WebApplication1.Data.Repositories.Interfaces;
using WebApplication1.Models.Domain;

namespace WebApplication1.Data.Repositories.Implementations;
    public class VehicleRepository : IVehicleRepository
    {
        private readonly PickTrashDbContext _context;
        private readonly ITransportCategoryRepository _transportCategoryRepository;

        public VehicleRepository(
            PickTrashDbContext context,
            ITransportCategoryRepository transportCategoryRepository)
        {
            _context = context;
            _transportCategoryRepository = transportCategoryRepository;
        }

        public async Task<Vehicle> RegisterVehicleAsync(Vehicle vehicle)
        {
            // Obtener la categoría según el peso
            var category = await _transportCategoryRepository.GetByCategoryNameAsync(vehicle.TransportCategory.Name);
            if (category == null)
                throw new BadRequestException("No se encontró una categoría válida para el peso especificado");

            vehicle.TransportCategoryId = category.Id;

            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();

            // Cargar la categoría para el retorno
            await _context.Entry(vehicle)
                .Reference(v => v.TransportCategory)
                .LoadAsync();

            return vehicle;
        }

        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            return await _context.Vehicles
                .Include(v => v.TransportCategory)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles
            .Include(v => v.TransportCategory)
            .ToListAsync();
        }

        public async Task<Vehicle?> GetVehicleWithCategoryAsync(int id)
        {
            return await _context.Vehicles
                .Include(v => v.TransportCategory)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Vehicles.AnyAsync(v => v.Id == id);
        }


    public async Task UpdateAsync(Vehicle vehicle)
    {
        var existingVehicle = await GetByIdAsync(vehicle.Id);
        if (existingVehicle == null)
            throw new NotFoundException($"Vehículo con ID {vehicle.Id} no encontrado");

        _context.Entry(existingVehicle).CurrentValues.SetValues(vehicle);
        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(int id)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
            throw new NotFoundException($"Vehículo con ID {id} no encontrado");

        _context.Remove(vehicle);
        await _context.SaveChangesAsync();
    }


    public async Task<Vehicle> AddAsync(Vehicle vehicle)
    {
        await _context.AddAsync(vehicle);
        await _context.SaveChangesAsync();

        // Cargar la categoría para el retorno
        await _context.Entry(vehicle)
            .Reference(v => v.TransportCategory)
            .LoadAsync();

        return vehicle;
    }



}

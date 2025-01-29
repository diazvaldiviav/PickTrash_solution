using Microsoft.EntityFrameworkCore;
using WebApplication1.Common.Exceptions;
using WebApplication1.Data.Repositories.Interfaces;
using WebApplication1.Models.Domain;

namespace WebApplication1.Data.Repositories.Implementations
{
    public class TransportCategoryRepository : ITransportCategoryRepository
    {
        private readonly PickTrashDbContext _context;

        public TransportCategoryRepository(PickTrashDbContext context)
        {
            _context = context;
        }

        public async Task<TransportCategory?> GetByCategoryNameAsync(string categoryName)
        {
            return await _context.TransportCategories
                .FirstOrDefaultAsync(tc => tc.Name.ToLower() == categoryName.ToLower());
        }

        public async Task<TransportCategory?> GetByWeightRangeAsync(decimal weight)
        {
            // Lógica para determinar la categoría según el peso
            // Estos rangos son ejemplos, ajústalos según tus necesidades
            return await _context.TransportCategories
                    .FirstOrDefaultAsync(tc =>
                      weight >= tc.MinWeight &&
                      weight <= tc.MaxWeight);
        }



        public async Task<TransportCategory?> GetByIdAsync(int id)
        {
            return await _context.TransportCategories
                .AsNoTracking()
                .Include(tc => tc.Vehicles)
                .FirstOrDefaultAsync(tc => tc.Id == id);
        }

        public async Task<IEnumerable<TransportCategory>> GetAllAsync()
        {
            return await _context.TransportCategories
                .Include(tc => tc.Vehicles)
                .ToListAsync();
        }

        public async Task<TransportCategory> AddAsync(TransportCategory category)
        {
            await _context.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task UpdateAsync(TransportCategory category)
        {
            var existingCategory = await GetByIdAsync(category.Id);
            if (existingCategory == null)
                throw new NotFoundException($"Categoría de transporte con ID {category.Id} no encontrada");

            _context.Entry(existingCategory).CurrentValues.SetValues(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category == null)
                throw new NotFoundException($"Categoría de transporte con ID {id} no encontrada");

            // Verificar si hay vehículos asociados
            if (category.Vehicles.Any())
                throw new BadRequestException("No se puede eliminar la categoría porque tiene vehículos asociados");

            _context.Remove(category);
            await _context.SaveChangesAsync();
        }

        public  async Task<bool> ExistsAsync(int id)
        {
            return await _context.TransportCategories.AnyAsync(tc => tc.Id == id);
        }

    }
}

using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Repositories.Interfaces;

namespace WebApplication1.Data.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly PickTrashDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(PickTrashDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            var result = await _dbSet.AddAsync(entity);
            return result.Entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(int id)
        {

            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return;


            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.FindAsync(id) != null;
        }
    }
}

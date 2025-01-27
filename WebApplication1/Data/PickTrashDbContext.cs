using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Configurations;

namespace WebApplication1.Data
{
    public class PickTrashDbContext : DbContext
    {
        public PickTrashDbContext(DbContextOptions<PickTrashDbContext> options)
       : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new DriverConfiguration());
            modelBuilder.ApplyConfiguration(new VehicleConfiguration());
            modelBuilder.ApplyConfiguration(new TransportCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new DriverVehicleConfiguration());
        }

    }
}

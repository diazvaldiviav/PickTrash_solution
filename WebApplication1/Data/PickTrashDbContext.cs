using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Configurations;
using WebApplication1.Models.Domain;

namespace WebApplication1.Data
{
    public class PickTrashDbContext : DbContext
    {
        public PickTrashDbContext(DbContextOptions<PickTrashDbContext> options)
       : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<TransportCategory> TransportCategories { get; set; }
        public DbSet<DriverVehicle> DriverVehicles { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestHistory> RequestHistories { get; set; }
        public DbSet<RequestMedia> RequestMedias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new DriverConfiguration());
            modelBuilder.ApplyConfiguration(new VehicleConfiguration());
            modelBuilder.ApplyConfiguration(new TransportCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new DriverVehicleConfiguration());
            modelBuilder.ApplyConfiguration(new RequestConfiguration());
            modelBuilder.ApplyConfiguration(new RequestHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new RequestMediaConfiguration());
        }

    }
}

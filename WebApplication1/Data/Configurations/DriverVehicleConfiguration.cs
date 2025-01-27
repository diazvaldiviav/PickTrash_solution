using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Domain;

namespace WebApplication1.Data.Configurations
{
    public class DriverVehicleConfiguration: IEntityTypeConfiguration<DriverVehicle>
    {
        public void Configure(EntityTypeBuilder<DriverVehicle> builder)
        {
            builder.HasKey(dv => new { dv.DriverId, dv.VehicleId });

            builder.HasOne(dv => dv.Driver)
                .WithMany(d => d.DriverVehicles)
                .HasForeignKey(dv => dv.DriverId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(dv => dv.Vehicle)
                .WithMany(v => v.DriverVehicles)
                .HasForeignKey(dv => dv.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

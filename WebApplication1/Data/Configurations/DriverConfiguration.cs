using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Domain;

namespace WebApplication1.Data.Configurations
{
    public class DriverConfiguration: IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Latitude)
                .HasPrecision(18, 6);

            builder.Property(d => d.Longitude)
                .HasPrecision(18, 6);

            // Relación uno a uno con User
            builder.HasOne(d => d.User)
                .WithOne(u => u.Driver)
                .HasForeignKey<Driver>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

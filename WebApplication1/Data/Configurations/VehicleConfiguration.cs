using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Domain;

namespace WebApplication1.Data.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Brand)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Weight)
                .HasPrecision(10, 2);

            // Relación con TransportCategory
            builder.HasOne(v => v.TransportCategory)
                .WithMany(tc => tc.Vehicles)
                .HasForeignKey(v => v.TransportCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

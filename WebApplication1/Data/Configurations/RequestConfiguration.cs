using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Domain;



namespace WebApplication1.Data.Configurations
{
    public class RequestConfiguration : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            // Clave primaria
            builder.HasKey(r => r.Id);

            // Propiedades
            builder.Property(r => r.PickupAddress)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.DropoffAddress)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.TrashType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.SpecialInstructions)
                .HasMaxLength(500);

            builder.Property(r => r.ProposedPrice)
                .HasPrecision(18, 2);

            builder.Property(r => r.FinalPrice)
                .HasPrecision(18, 2);

            // Relaciones
            builder.HasOne(r => r.Client)
                .WithMany(c => c.Requests)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Driver)
                .WithMany(d => d.Requests)
                .HasForeignKey(r => r.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.ScheduledDate);
            builder.HasIndex(r => new { r.ClientId, r.Status });
            builder.HasIndex(r => new { r.DriverId, r.Status });
        }
    }
}

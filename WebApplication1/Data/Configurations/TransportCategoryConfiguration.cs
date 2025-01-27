using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Models.Domain;

namespace WebApplication1.Data.Configurations
{
    public class TransportCategoryConfiguration: IEntityTypeConfiguration<TransportCategory>
    {
        public void Configure(EntityTypeBuilder<TransportCategory> builder)
        {
            builder.HasKey(tc => tc.Id);

            builder.Property(tc => tc.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(tc => tc.MinWeight)
                .HasPrecision(10, 2);

            builder.Property(tc => tc.MaxWeight)
                .HasPrecision(10, 2);
        }
    }
}

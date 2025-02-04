using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Domain;

namespace WebApplication1.Data.Configurations
{
    public class RequestHistoryConfiguration : IEntityTypeConfiguration<RequestHistory>
    {
        public void Configure(EntityTypeBuilder<RequestHistory> builder)
        {
            builder.HasKey(rh => rh.Id);

            builder.Property(rh => rh.ChangeReason)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(rh => rh.PreviousPrice)
                .HasPrecision(18, 2);

            builder.Property(rh => rh.NewPrice)
                .HasPrecision(18, 2);

            builder.HasOne(rh => rh.Request)
                .WithMany()
                .HasForeignKey(rh => rh.RequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

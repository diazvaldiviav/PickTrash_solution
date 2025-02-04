using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Domain;

namespace WebApplication1.Data.Configurations
{
    public class RequestMediaConfiguration : IEntityTypeConfiguration<RequestMedia>
    {
        public void Configure(EntityTypeBuilder<RequestMedia> builder)
        {
            builder.HasKey(rm => rm.Id);

            builder.Property(rm => rm.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(rm => rm.FileUrl)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(rm => rm.FileType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(rm => rm.Description)
                .HasMaxLength(500);

            builder.HasOne(rm => rm.Request)
                .WithMany()
                .HasForeignKey(rm => rm.RequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

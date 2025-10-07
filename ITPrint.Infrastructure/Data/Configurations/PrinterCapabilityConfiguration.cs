using ITPrint.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITPrint.Infrastructure.Data.Configurations;

public class PrinterCapabilityConfiguration : IEntityTypeConfiguration<PrinterCapability>
{
    public void Configure(EntityTypeBuilder<PrinterCapability> builder)
    {
        builder.ToTable("PrinterCapabilities"); 
        
        builder.HasKey(pc => pc.Id);

        builder.Property(pc => pc.PaperFormat)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(pc => pc.MaxCopies)
            .HasDefaultValue(1);

        builder.Property(pc => pc.MaxPagesPerJob)
            .HasDefaultValue(1000);

        builder.HasIndex(pc => new { pc.PrinterId, pc.PaperFormat })
            .IsUnique();
    }
}
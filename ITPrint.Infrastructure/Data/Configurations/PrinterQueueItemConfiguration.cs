using ITPrint.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITPrint.Infrastructure.Data.Configurations;

public class PrinterQueueItemConfiguration : IEntityTypeConfiguration<PrinterQueueItem>
{
    public void Configure(EntityTypeBuilder<PrinterQueueItem> builder)
    {
        builder.ToTable("PrinterQueueItems");
        
        builder.HasKey(qi => qi.Id);

        builder.Property(qi => qi.QueuePosition)
            .IsRequired();

        builder.Property(qi => qi.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(qi => qi.RetryCount)
            .HasDefaultValue(0);

        builder.Property(qi => qi.MaxRetries)
            .HasDefaultValue(3);

        builder.Property(qi => qi.AddedToQueueAt)
            .IsRequired();

        builder.Property(qi => qi.CupsJobId)
            .HasMaxLength(100);

        builder.Property(qi => qi.ErrorMessage)
            .HasMaxLength(2000);
        
        builder.HasIndex(qi => qi.PrinterId);
        builder.HasIndex(qi => qi.Status);
        builder.HasIndex(qi => new { qi.PrinterId, qi.QueuePosition });
    }
}
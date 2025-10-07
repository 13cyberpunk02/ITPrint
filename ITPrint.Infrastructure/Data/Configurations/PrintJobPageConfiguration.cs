using ITPrint.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITPrint.Infrastructure.Data.Configurations;

public class PrintJobPageConfiguration : IEntityTypeConfiguration<PrintJobPage>
{
    public void Configure(EntityTypeBuilder<PrintJobPage> builder)
    {
        builder.ToTable("PrintJobPages");
        
        builder.HasKey(pp => pp.Id);

        builder.Property(pp => pp.PageNumber)
            .IsRequired();

        builder.Property(pp => pp.PaperFormat)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(pp => pp.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(pp => pp.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(pp => pp.CreatedAt)
            .IsRequired();

        builder.Property(pp => pp.ErrorMessage)
            .HasMaxLength(2000);

        builder.HasIndex(pp => pp.PrintJobId);
        builder.HasIndex(pp => pp.Status);
        builder.HasIndex(pp => pp.AssignedPrinterId);
        builder.HasIndex(pp => new { pp.PrintJobId, pp.PageNumber })
            .IsUnique();

        builder.HasOne(pp => pp.QueueItem)
            .WithOne(qi => qi.PrintJobPage)
            .HasForeignKey<PrinterQueueItem>(qi => qi.PrintJobPageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
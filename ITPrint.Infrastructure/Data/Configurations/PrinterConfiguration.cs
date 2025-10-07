using ITPrint.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITPrint.Infrastructure.Data.Configurations;

public class PrinterConfiguration : IEntityTypeConfiguration<Printer>
{
    public void Configure(EntityTypeBuilder<Printer> builder)
    {
        builder.ToTable("Printers");
        
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Model)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.CupsName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Location)
            .HasMaxLength(500);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();

        
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasIndex(p => p.CupsName)
            .IsUnique();

        builder.HasIndex(p => p.Status);

        builder.HasMany(p => p.Capabilities)
            .WithOne(pc => pc.Printer)
            .HasForeignKey(pc => pc.PrinterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.AssignedPages)
            .WithOne(pp => pp.AssignedPrinter)
            .HasForeignKey(pp => pp.AssignedPrinterId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(p => p.QueueItems)
            .WithOne(qi => qi.Printer)
            .HasForeignKey(qi => qi.PrinterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
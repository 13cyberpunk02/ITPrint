using ITPrint.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITPrint.Infrastructure.Data.Configurations;

public class PrintStatisticsConfiguration : IEntityTypeConfiguration<PrintStatistics>
{
    public void Configure(EntityTypeBuilder<PrintStatistics> builder)
    {
        builder.ToTable("PrintStatistics");
        
        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.Date)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(ps => ps.PaperFormat)
            .HasConversion<string>();

        builder.Property(ps => ps.CreatedAt)
            .IsRequired();

        builder.HasIndex(ps => ps.UserId);
        builder.HasIndex(ps => ps.PrinterId);
        builder.HasIndex(ps => ps.Date);
        builder.HasIndex(ps => new { ps.UserId, ps.Date });
        builder.HasIndex(ps => new { ps.PrinterId, ps.Date });

        builder.HasOne(ps => ps.User)
            .WithMany()
            .HasForeignKey(ps => ps.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(ps => ps.Printer)
            .WithMany()
            .HasForeignKey(ps => ps.PrinterId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
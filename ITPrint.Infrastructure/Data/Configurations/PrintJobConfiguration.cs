using ITPrint.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITPrint.Infrastructure.Data.Configurations;

public class PrintJobConfiguration : IEntityTypeConfiguration<PrintJob>
{
    public void Configure(EntityTypeBuilder<PrintJob> builder)
    {
        builder.ToTable("PrintJobs");
        
        builder.HasKey(pj => pj.Id);

        builder.Property(pj => pj.OriginalFileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(pj => pj.OriginalFilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(pj => pj.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(pj => pj.Copies)
            .HasDefaultValue(1);

        builder.Property(pj => pj.CreatedAt)
            .IsRequired();

        builder.Property(pj => pj.ErrorMessage)
            .HasMaxLength(2000);

        builder.HasIndex(pj => pj.UserId);
        builder.HasIndex(pj => pj.Status);
        builder.HasIndex(pj => pj.CreatedAt);

        builder.HasMany(pj => pj.Pages)
            .WithOne(pp => pp.PrintJob)
            .HasForeignKey(pp => pp.PrintJobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
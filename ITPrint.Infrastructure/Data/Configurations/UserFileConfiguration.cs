using ITPrint.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITPrint.Infrastructure.Data.Configurations;

public class UserFileConfiguration : IEntityTypeConfiguration<UserFile>
{
    public void Configure(EntityTypeBuilder<UserFile> builder)
    {
        builder.ToTable("UserFiles");
        
        builder.HasKey(uf => uf.Id);

        builder.Property(uf => uf.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(uf => uf.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(uf => uf.MimeType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(uf => uf.UploadedAt)
            .IsRequired();
        
        builder.HasIndex(uf => uf.UserId);
        builder.HasIndex(uf => uf.UploadedAt);
        builder.HasIndex(uf => uf.IsDeleted);
    }
}

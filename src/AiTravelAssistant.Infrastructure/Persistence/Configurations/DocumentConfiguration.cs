using AiTravelAssistant.Domain.Entities;
using AiTravelAssistant.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiTravelAssistant.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core table and column configuration for the <see cref="Document"/> entity.
/// </summary>
public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Destination)
            .HasMaxLength(200);

        builder.Property(d => d.Language)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(d => d.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(d => d.FailureReason)
            .HasMaxLength(2000);

        builder.Property(d => d.UploadedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.StoragePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(d => d.UploadedAt).IsRequired();
        builder.Property(d => d.CreatedAt).IsRequired();
        builder.Property(d => d.UpdatedAt).IsRequired();

        builder.HasIndex(d => d.Status);
        builder.HasIndex(d => d.Category);
    }
}

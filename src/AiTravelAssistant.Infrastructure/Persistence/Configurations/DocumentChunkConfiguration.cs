using AiTravelAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiTravelAssistant.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core table and column configuration for the <see cref="DocumentChunk"/> entity.
/// </summary>
public class DocumentChunkConfiguration : IEntityTypeConfiguration<DocumentChunk>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<DocumentChunk> builder)
    {
        builder.ToTable("document_chunks");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content)
            .IsRequired();

        builder.Property(c => c.PageReference)
            .HasMaxLength(100);

        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();

        builder.HasIndex(c => c.DocumentId);
    }
}

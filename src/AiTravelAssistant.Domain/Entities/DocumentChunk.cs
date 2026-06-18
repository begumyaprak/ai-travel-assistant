using AiTravelAssistant.Domain.Common;

namespace AiTravelAssistant.Domain.Entities;

/// <summary>
/// Represents a parsed and indexed text chunk derived from a <see cref="Document"/>.
/// </summary>
public class DocumentChunk : BaseEntity
{
    /// <summary>Gets the identifier of the parent document this chunk belongs to.</summary>
    public Guid DocumentId { get; private set; }

    /// <summary>Gets the extracted text content of this chunk.</summary>
    public string Content { get; private set; } = default!;

    /// <summary>Gets the zero-based sequential index of this chunk within the parent document.</summary>
    public int ChunkIndex { get; private set; }

    /// <summary>Gets the optional page reference indicating where this chunk originated in the source document.</summary>
    public string? PageReference { get; private set; }

    private DocumentChunk() { }

    /// <summary>
    /// Creates a new <see cref="DocumentChunk"/> with the specified content and position metadata.
    /// </summary>
    /// <param name="documentId">The identifier of the parent document.</param>
    /// <param name="content">The extracted text content of the chunk.</param>
    /// <param name="chunkIndex">The zero-based index of this chunk within the document.</param>
    /// <param name="pageReference">An optional page reference indicating the chunk's source location.</param>
    /// <returns>A newly created <see cref="DocumentChunk"/> instance.</returns>
    public static DocumentChunk Create(
        Guid documentId,
        string content,
        int chunkIndex,
        string? pageReference)
    {
        var now = DateTime.UtcNow;
        return new DocumentChunk
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            Content = content,
            ChunkIndex = chunkIndex,
            PageReference = pageReference,
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}

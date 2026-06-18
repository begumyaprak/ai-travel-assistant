using AiTravelAssistant.Domain.Entities;

namespace AiTravelAssistant.Application.Interfaces.Repositories;

/// <summary>
/// Defines persistence operations for <see cref="DocumentChunk"/> entities.
/// </summary>
public interface IDocumentChunkRepository
{
    /// <summary>
    /// Persists a collection of chunks to the store in a single operation.
    /// </summary>
    /// <param name="chunks">The chunks to insert.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    Task AddRangeAsync(IEnumerable<DocumentChunk> chunks, CancellationToken cancellationToken);

    /// <summary>
    /// Returns all chunks belonging to the specified document, ordered by chunk index.
    /// </summary>
    /// <param name="documentId">The identifier of the parent document.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A read-only list of <see cref="DocumentChunk"/> records.</returns>
    Task<IReadOnlyList<DocumentChunk>> GetByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken);
}

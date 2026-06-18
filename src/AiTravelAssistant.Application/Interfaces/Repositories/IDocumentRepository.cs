using AiTravelAssistant.Domain.Entities;

namespace AiTravelAssistant.Application.Interfaces.Repositories;

/// <summary>
/// Defines persistence operations for the <see cref="Document"/> aggregate.
/// </summary>
public interface IDocumentRepository
{
    /// <summary>
    /// Retrieves a document by its unique identifier, or <see langword="null"/> if not found.
    /// </summary>
    /// <param name="id">The document identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The matching <see cref="Document"/>, or <see langword="null"/>.</returns>
    Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Returns all documents ordered by upload date descending.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A read-only list of all <see cref="Document"/> records.</returns>
    Task<IReadOnlyList<Document>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Persists a new document to the store.
    /// </summary>
    /// <param name="document">The document to add.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    Task AddAsync(Document document, CancellationToken cancellationToken);

    /// <summary>
    /// Applies changes to an existing document in the store.
    /// </summary>
    /// <param name="document">The document with updated state.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    Task UpdateAsync(Document document, CancellationToken cancellationToken);
}

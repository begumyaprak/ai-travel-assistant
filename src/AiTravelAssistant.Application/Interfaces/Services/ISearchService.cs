using AiTravelAssistant.Application.QA.Models;

namespace AiTravelAssistant.Application.Interfaces.Services;

/// <summary>
/// Provides vector and semantic search operations against the document index.
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Performs a hybrid vector and semantic search and returns the most relevant document chunks.
    /// </summary>
    /// <param name="queryVector">The embedding vector of the user's question.</param>
    /// <param name="query">The raw question text used for semantic re-ranking.</param>
    /// <param name="topK">The maximum number of results to return.</param>
    /// <param name="destination">An optional destination filter to narrow results.</param>
    /// <param name="category">An optional category filter to narrow results.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A read-only list of matching <see cref="SearchResult"/> items ordered by relevance.</returns>
    Task<IReadOnlyList<SearchResult>> SearchAsync(
        float[] queryVector,
        string query,
        int topK,
        string? destination,
        string? category,
        CancellationToken cancellationToken);

    /// <summary>
    /// Indexes a collection of document chunks into the search index.
    /// </summary>
    /// <param name="chunks">The chunks to index, each including its embedding vector.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    Task IndexChunksAsync(IEnumerable<ChunkIndexRequest> chunks, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes all indexed chunks associated with the specified document.
    /// </summary>
    /// <param name="documentId">The identifier of the document whose chunks should be removed.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    Task DeleteByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken);
}

/// <summary>
/// Represents the data required to index a single document chunk into the search index.
/// </summary>
/// <param name="Id">The unique search-index identifier for this chunk (format: <c>{documentId}_{chunkIndex}</c>).</param>
/// <param name="DocumentId">The identifier of the parent document.</param>
/// <param name="FileName">The original file name of the parent document.</param>
/// <param name="Content">The text content of the chunk.</param>
/// <param name="ChunkIndex">The zero-based index of this chunk within its document.</param>
/// <param name="PageReference">An optional page reference indicating the chunk's location in the source document.</param>
/// <param name="Category">The category of the parent document.</param>
/// <param name="Destination">The optional destination associated with the parent document.</param>
/// <param name="ContentVector">The embedding vector of the chunk content.</param>
public record ChunkIndexRequest(
    string Id,
    Guid DocumentId,
    string FileName,
    string Content,
    int ChunkIndex,
    string? PageReference,
    string Category,
    string? Destination,
    float[] ContentVector);

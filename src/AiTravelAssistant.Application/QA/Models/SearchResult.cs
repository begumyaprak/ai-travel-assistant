namespace AiTravelAssistant.Application.QA.Models;

/// <summary>
/// Represents a single document chunk returned by the vector/semantic search.
/// </summary>
/// <param name="DocumentId">The identifier of the document this chunk belongs to.</param>
/// <param name="FileName">The original file name of the source document.</param>
/// <param name="ChunkContent">The extracted text content of the chunk.</param>
/// <param name="PageReference">An optional page reference indicating the chunk's location in the source document.</param>
/// <param name="RelevanceScore">The relevance score assigned by the search index (higher is more relevant).</param>
public record SearchResult(
    Guid DocumentId,
    string FileName,
    string ChunkContent,
    string? PageReference,
    double RelevanceScore);

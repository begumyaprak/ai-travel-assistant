using AiTravelAssistant.Application.Interfaces.Repositories;
using AiTravelAssistant.Application.Interfaces.Services;
using AiTravelAssistant.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AiTravelAssistant.Infrastructure.Jobs;

/// <summary>
/// Hangfire background job that parses, chunks, embeds, and indexes a document after upload.
/// </summary>
/// <remarks>
/// Chunks are sized at 2 000 characters with a 200-character overlap to preserve context across boundaries.
/// Hangfire retries this job up to three times on failure; status is updated to <c>Failed</c> on the final attempt.
/// </remarks>
public class DocumentProcessingJob
{
    private const int ChunkSize = 2000;
    private const int ChunkOverlap = 200;

    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentChunkRepository _chunkRepository;
    private readonly IDocumentStorageService _storageService;
    private readonly IDocumentParserService _parserService;
    private readonly IEmbeddingService _embeddingService;
    private readonly ISearchService _searchService;
    private readonly ILogger<DocumentProcessingJob> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="DocumentProcessingJob"/> with all required services.
    /// </summary>
    /// <param name="documentRepository">The repository used to read and update the document record.</param>
    /// <param name="chunkRepository">The repository used to persist extracted chunks.</param>
    /// <param name="storageService">The service used to read the stored file.</param>
    /// <param name="parserService">The service used to extract plain text from the file.</param>
    /// <param name="embeddingService">The service used to generate embedding vectors for each chunk.</param>
    /// <param name="searchService">The service used to index chunks into Azure AI Search.</param>
    /// <param name="logger">The logger used to emit processing lifecycle events.</param>
    public DocumentProcessingJob(
        IDocumentRepository documentRepository,
        IDocumentChunkRepository chunkRepository,
        IDocumentStorageService storageService,
        IDocumentParserService parserService,
        IEmbeddingService embeddingService,
        ISearchService searchService,
        ILogger<DocumentProcessingJob> logger)
    {
        _documentRepository = documentRepository;
        _chunkRepository = chunkRepository;
        _storageService = storageService;
        _parserService = parserService;
        _embeddingService = embeddingService;
        _searchService = searchService;
        _logger = logger;
    }

    /// <summary>
    /// Executes the full document processing pipeline: parse, chunk, embed, index, and update status.
    /// </summary>
    /// <param name="documentId">The identifier of the document to process.</param>
    public async Task ProcessAsync(Guid documentId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId, CancellationToken.None);
        if (document is null)
        {
            _logger.LogWarning("Document {DocumentId} not found, skipping processing", documentId);
            return;
        }

        try
        {
            document.MarkAsProcessing();
            await _documentRepository.UpdateAsync(document, CancellationToken.None);

            await using var fileStream = await _storageService.ReadAsync(document.StoragePath, CancellationToken.None);
            var rawText = await _parserService.ParseAsync(fileStream, document.FileName, CancellationToken.None);

            var textChunks = SplitIntoChunks(rawText).ToList();

            var domainChunks = new List<DocumentChunk>();
            var indexRequests = new List<ChunkIndexRequest>();

            foreach (var (content, index) in textChunks)
            {
                var embedding = await _embeddingService.GenerateAsync(content, CancellationToken.None);

                domainChunks.Add(DocumentChunk.Create(document.Id, content, index, pageReference: null));

                indexRequests.Add(new ChunkIndexRequest(
                    Id: $"{document.Id}_{index}",
                    DocumentId: document.Id,
                    FileName: document.FileName,
                    Content: content,
                    ChunkIndex: index,
                    PageReference: null,
                    Category: document.Category,
                    Destination: document.Destination,
                    ContentVector: embedding));
            }

            await _chunkRepository.AddRangeAsync(domainChunks, CancellationToken.None);
            await _searchService.IndexChunksAsync(indexRequests, CancellationToken.None);

            document.MarkAsProcessed();
            await _documentRepository.UpdateAsync(document, CancellationToken.None);

            _logger.LogInformation("Document {DocumentId} processed successfully — {ChunkCount} chunks", documentId, textChunks.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process document {DocumentId}", documentId);
            document.MarkAsFailed(ex.Message);
            await _documentRepository.UpdateAsync(document, CancellationToken.None);
            throw;
        }
    }

    private static IEnumerable<(string Content, int Index)> SplitIntoChunks(string text)
    {
        var position = 0;
        var chunkIndex = 0;

        while (position < text.Length)
        {
            var end = Math.Min(position + ChunkSize, text.Length);
            yield return (text[position..end], chunkIndex++);

            if (end == text.Length) break;
            position += ChunkSize - ChunkOverlap;
        }
    }
}

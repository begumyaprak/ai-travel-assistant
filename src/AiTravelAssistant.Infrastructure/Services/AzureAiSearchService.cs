using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using AiTravelAssistant.Application.Interfaces.Services;
using AiTravelAssistant.Application.QA.Models;
using AiTravelAssistant.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace AiTravelAssistant.Infrastructure.Services;

/// <summary>
/// Implements <see cref="ISearchService"/> using Azure AI Search with combined vector and semantic search.
/// </summary>
public class AzureAiSearchService : ISearchService
{
    private readonly SearchClient _searchClient;
    private readonly AzureSearchSettings _settings;

    /// <summary>
    /// Initializes a new instance of <see cref="AzureAiSearchService"/> and configures the underlying search client.
    /// </summary>
    /// <param name="options">The Azure AI Search configuration options.</param>
    public AzureAiSearchService(IOptions<AzureSearchSettings> options)
    {
        _settings = options.Value;
        _searchClient = new SearchClient(
            new Uri(_settings.Endpoint),
            _settings.IndexName,
            new AzureKeyCredential(_settings.ApiKey));
    }

    /// <summary>
    /// Executes a hybrid vector + semantic search against the travel document index.
    /// </summary>
    /// <param name="queryVector">The embedding vector of the user's question.</param>
    /// <param name="query">The raw question text used for semantic re-ranking when a semantic configuration is set.</param>
    /// <param name="topK">The maximum number of results to return.</param>
    /// <param name="destination">An optional OData destination filter.</param>
    /// <param name="category">An optional OData category filter.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A list of <see cref="SearchResult"/> items ordered by reranker or vector score descending.</returns>
    public async Task<IReadOnlyList<SearchResult>> SearchAsync(
        float[] queryVector,
        string query,
        int topK,
        string? destination,
        string? category,
        CancellationToken cancellationToken)
    {
        var options = new SearchOptions { Size = topK };

        var vectorQuery = new VectorizedQuery(queryVector) { KNearestNeighborsCount = topK };
        vectorQuery.Fields.Add("contentVector");
        options.VectorSearch = new VectorSearchOptions();
        options.VectorSearch.Queries.Add(vectorQuery);

        var filters = new List<string>();
        if (!string.IsNullOrEmpty(destination)) filters.Add($"destination eq '{destination}'");
        if (!string.IsNullOrEmpty(category)) filters.Add($"category eq '{category}'");
        if (filters.Count > 0) options.Filter = string.Join(" and ", filters);

        if (!string.IsNullOrEmpty(_settings.SemanticConfigurationName))
        {
            options.QueryType = SearchQueryType.Semantic;
            options.SemanticSearch = new SemanticSearchOptions
            {
                SemanticConfigurationName = _settings.SemanticConfigurationName
            };
        }

        var response = await _searchClient.SearchAsync<TravelDocumentSearchModel>(query, options, cancellationToken);

        var results = new List<SearchResult>();
        await foreach (var result in response.Value.GetResultsAsync())
        {
            var doc = result.Document;
            var score = result.SemanticSearch?.RerankerScore ?? result.Score ?? 0.0;
            results.Add(new SearchResult(
                Guid.Parse(doc.DocumentId),
                doc.FileName,
                doc.Content,
                doc.PageReference,
                score));
        }

        return results;
    }

    /// <summary>
    /// Uploads a batch of document chunks to the Azure AI Search index.
    /// </summary>
    /// <param name="chunks">The chunks to index, including their content vectors.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    public async Task IndexChunksAsync(IEnumerable<ChunkIndexRequest> chunks, CancellationToken cancellationToken)
    {
        var documents = chunks.Select(c => new TravelDocumentSearchModel
        {
            Id = c.Id,
            DocumentId = c.DocumentId.ToString(),
            FileName = c.FileName,
            Content = c.Content,
            ChunkIndex = c.ChunkIndex,
            PageReference = c.PageReference,
            Category = c.Category,
            Destination = c.Destination,
            ContentVector = c.ContentVector
        });

        await _searchClient.UploadDocumentsAsync(documents, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Removes all indexed chunks belonging to the specified document from the search index.
    /// </summary>
    /// <param name="documentId">The identifier of the document whose chunks should be deleted.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    public async Task DeleteByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken)
    {
        var options = new SearchOptions
        {
            Filter = $"documentId eq '{documentId}'",
            Size = 1000
        };
        options.Select.Add("id");

        var response = await _searchClient.SearchAsync<TravelDocumentSearchModel>("*", options, cancellationToken);

        var ids = new List<string>();
        await foreach (var result in response.Value.GetResultsAsync())
            ids.Add(result.Document.Id);

        if (ids.Count > 0)
            await _searchClient.DeleteDocumentsAsync("id", ids, cancellationToken: cancellationToken);
    }

    private class TravelDocumentSearchModel
    {
        [JsonPropertyName("id")] public string Id { get; set; } = default!;
        [JsonPropertyName("documentId")] public string DocumentId { get; set; } = default!;
        [JsonPropertyName("fileName")] public string FileName { get; set; } = default!;
        [JsonPropertyName("content")] public string Content { get; set; } = default!;
        [JsonPropertyName("chunkIndex")] public int ChunkIndex { get; set; }
        [JsonPropertyName("pageReference")] public string? PageReference { get; set; }
        [JsonPropertyName("category")] public string Category { get; set; } = default!;
        [JsonPropertyName("destination")] public string? Destination { get; set; }
        [JsonPropertyName("contentVector")] public float[] ContentVector { get; set; } = default!;
    }
}

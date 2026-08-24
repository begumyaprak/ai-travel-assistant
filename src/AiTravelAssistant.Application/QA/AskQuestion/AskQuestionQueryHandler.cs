using System.Security.Cryptography;
using System.Text;
using AiTravelAssistant.Application.Common;
using AiTravelAssistant.Application.Interfaces.Services;
using AiTravelAssistant.Application.QA.Models;
using MediatR;

namespace AiTravelAssistant.Application.QA.AskQuestion;

/// <summary>
/// Handles <see cref="AskQuestionQuery"/> by executing the full RAG pipeline: cache check, embedding, vector search, and LLM completion.
/// </summary>
public class AskQuestionQueryHandler : IRequestHandler<AskQuestionQuery, Result<AskQuestionResponse>>
{
    private const int RetrievalCandidateCount = 50;
    private const int ContextResultCount = 5;
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(1);
    private const string NotFoundAnswer = "The uploaded documents do not contain enough information to answer this question.";

    private readonly IEmbeddingService _embeddingService;
    private readonly ISearchService _searchService;
    private readonly ICompletionService _completionService;
    private readonly ICacheService _cacheService;

    /// <summary>
    /// Initializes a new instance of <see cref="AskQuestionQueryHandler"/>.
    /// </summary>
    /// <param name="embeddingService">The service used to generate the query embedding vector.</param>
    /// <param name="searchService">The service used to perform vector and semantic search.</param>
    /// <param name="completionService">The LLM service used to generate grounded answers.</param>
    /// <param name="cacheService">The cache service used to store and retrieve completed responses.</param>
    public AskQuestionQueryHandler(
        IEmbeddingService embeddingService,
        ISearchService searchService,
        ICompletionService completionService,
        ICacheService cacheService)
    {
        _embeddingService = embeddingService;
        _searchService = searchService;
        _completionService = completionService;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Executes the RAG pipeline and returns either a cached or freshly generated answer.
    /// </summary>
    /// <param name="request">The query containing the user's question and optional filters.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A successful result containing the answer, sources, and confidence level.</returns>
    public async Task<Result<AskQuestionResponse>> Handle(AskQuestionQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = BuildCacheKey(request);

        var cached = await _cacheService.GetAsync<AskQuestionResponse>(cacheKey, cancellationToken);
        if (cached is not null)
            return Result<AskQuestionResponse>.Success(cached with { FromCache = true });

        var queryVector = await _embeddingService.GenerateAsync(request.Question, cancellationToken);

        var results = await _searchService.SearchAsync(
            queryVector,
            request.Question,
            RetrievalCandidateCount,
            request.Destination,
            request.Category,
            cancellationToken);

        if (results.Count == 0)
        {
            return Result<AskQuestionResponse>.Success(new AskQuestionResponse(
                NotFoundAnswer,
                Array.Empty<SearchResult>(),
                FromCache: false,
                ConfidenceLevel.NotFound));
        }

        // Semantic ranking needs a broad candidate set, but the completion model should
        // receive only the highest-ranked excerpts to keep its context focused.
        var contextResults = results.Take(ContextResultCount).ToList();
        var confidence = DetermineConfidence(contextResults);

        if (confidence == ConfidenceLevel.NotFound)
        {
            return Result<AskQuestionResponse>.Success(new AskQuestionResponse(
                NotFoundAnswer,
                contextResults,
                FromCache: false,
                ConfidenceLevel.NotFound));
        }

        var context = BuildContext(contextResults);
        var systemPrompt = BuildSystemPrompt();
        var userPrompt = BuildUserPrompt(request.Question, context);

        var answer = await _completionService.CompleteAsync(systemPrompt, userPrompt, cancellationToken);

        var response = new AskQuestionResponse(answer, contextResults, FromCache: false, confidence);

        await _cacheService.SetAsync(cacheKey, response, CacheTtl, cancellationToken);

        return Result<AskQuestionResponse>.Success(response);
    }

    private static string BuildCacheKey(AskQuestionQuery request)
    {
        var normalizedQuestion = NormalizeCacheValue(request.Question);
        var normalizedDestination = NormalizeCacheValue(request.Destination);
        var normalizedCategory = NormalizeCacheValue(request.Category);
        var cacheInput = $"question:{normalizedQuestion}\ndestination:{normalizedDestination}\ncategory:{normalizedCategory}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(cacheInput));
        return $"qa:v2:{Convert.ToHexString(hash)}";
    }

    private static string NormalizeCacheValue(string? value) =>
        value?.Trim().ToLowerInvariant() ?? string.Empty;

    private static ConfidenceLevel DetermineConfidence(IReadOnlyList<SearchResult> results)
    {
        var semanticScores = results
            .Where(r => r.IsSemanticScore)
            .Select(r => r.RelevanceScore)
            .ToList();

        // Hybrid RRF scores are rank-fusion values, not calibrated relevance values.
        // Without semantic scores we can rank sources, but cannot derive a reliable
        // relevance threshold from that score, so report a conservative confidence.
        if (semanticScores.Count == 0)
            return ConfidenceLevel.Low;

        var topScore = semanticScores.Max();
        return topScore switch
        {
            >= 3.0 => ConfidenceLevel.High,
            >= 2.0 => ConfidenceLevel.Medium,
            >= 1.0 => ConfidenceLevel.Low,
            _ => ConfidenceLevel.NotFound
        };
    }

    private static string BuildContext(IReadOnlyList<SearchResult> results)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < results.Count; i++)
        {
            var r = results[i];
            sb.AppendLine($"[Source {i + 1}] {r.FileName}{(r.PageReference is not null ? $" (page {r.PageReference})" : string.Empty)}");
            sb.AppendLine(r.ChunkContent);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private static string BuildSystemPrompt() =>
        "You are a travel assistant. Answer the user's question based strictly on the provided document excerpts. " +
        "If the documents do not contain sufficient information, say so explicitly. Never invent or infer information not present in the sources.";

    private static string BuildUserPrompt(string question, string context) =>
        $"Document excerpts:\n{context}\nQuestion: {question}";
}

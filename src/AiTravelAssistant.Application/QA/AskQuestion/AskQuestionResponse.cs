using AiTravelAssistant.Application.QA.Models;

namespace AiTravelAssistant.Application.QA.AskQuestion;

/// <summary>
/// Indicates how confidently the answer is grounded in the retrieved document chunks.
/// </summary>
public enum ConfidenceLevel
{
    /// <summary>The top semantic reranker score is 3.0 or above.</summary>
    High,

    /// <summary>The top semantic reranker score is between 2.0 and 2.99.</summary>
    Medium,

    /// <summary>The top semantic reranker score is between 1.0 and 1.99, or semantic ranking is unavailable.</summary>
    Low,

    /// <summary>No semantic reranker score met the minimum relevance threshold; the question could not be answered.</summary>
    NotFound
}

/// <summary>
/// The response returned by the RAG pipeline for a user question.
/// </summary>
/// <param name="Answer">The generated answer text, grounded in the retrieved document chunks.</param>
/// <param name="Sources">The document chunks used as evidence to formulate the answer.</param>
/// <param name="FromCache">Indicates whether this response was served from the Redis cache.</param>
/// <param name="Confidence">The confidence level of the answer based on retrieval scores.</param>
public record AskQuestionResponse(
    string Answer,
    IReadOnlyList<SearchResult> Sources,
    bool FromCache,
    ConfidenceLevel Confidence);

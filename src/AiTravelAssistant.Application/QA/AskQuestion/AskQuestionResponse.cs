using AiTravelAssistant.Application.QA.Models;

namespace AiTravelAssistant.Application.QA.AskQuestion;

/// <summary>
/// Indicates how confidently the answer is grounded in the retrieved document chunks.
/// </summary>
public enum ConfidenceLevel
{
    /// <summary>The top retrieved chunk has a relevance score of 0.85 or above.</summary>
    High,

    /// <summary>The top retrieved chunk has a relevance score between 0.70 and 0.84.</summary>
    Medium,

    /// <summary>The top retrieved chunk has a relevance score between 0.50 and 0.69.</summary>
    Low,

    /// <summary>No chunks met the minimum relevance threshold; the question could not be answered.</summary>
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

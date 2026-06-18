namespace AiTravelAssistant.Application.Interfaces.Services;

/// <summary>
/// Generates dense vector embeddings for text using a language model.
/// </summary>
public interface IEmbeddingService
{
    /// <summary>
    /// Generates a floating-point embedding vector for the provided text.
    /// </summary>
    /// <param name="text">The input text to embed.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A float array representing the embedding vector.</returns>
    Task<float[]> GenerateAsync(string text, CancellationToken cancellationToken);
}

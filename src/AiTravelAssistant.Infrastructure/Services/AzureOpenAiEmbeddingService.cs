using Azure;
using Azure.AI.OpenAI;
using AiTravelAssistant.Application.Interfaces.Services;
using AiTravelAssistant.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;

namespace AiTravelAssistant.Infrastructure.Services;

/// <summary>
/// Implements <see cref="IEmbeddingService"/> using the Azure OpenAI embeddings endpoint.
/// </summary>
public class AzureOpenAiEmbeddingService : IEmbeddingService
{
    private readonly EmbeddingClient _client;

    /// <summary>
    /// Initializes a new instance of <see cref="AzureOpenAiEmbeddingService"/> using the provided settings.
    /// </summary>
    /// <param name="options">The Azure OpenAI configuration options.</param>
    public AzureOpenAiEmbeddingService(IOptions<AzureOpenAiSettings> options)
    {
        var settings = options.Value;
        var azureClient = new AzureOpenAIClient(
            new Uri(settings.Endpoint),
            new AzureKeyCredential(settings.ApiKey));
        _client = azureClient.GetEmbeddingClient(settings.DeploymentEmbedding);
    }

    /// <summary>
    /// Calls the Azure OpenAI embeddings API and returns the resulting vector for the given text.
    /// </summary>
    /// <param name="text">The input text to embed.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A float array representing the embedding vector.</returns>
    public async Task<float[]> GenerateAsync(string text, CancellationToken cancellationToken)
    {
        var result = await _client.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken);
        return result.Value.ToFloats().ToArray();
    }
}

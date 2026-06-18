using Azure;
using Azure.AI.OpenAI;
using AiTravelAssistant.Application.Interfaces.Services;
using AiTravelAssistant.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace AiTravelAssistant.Infrastructure.Services;

/// <summary>
/// Implements <see cref="ICompletionService"/> using the Azure OpenAI chat completions endpoint.
/// </summary>
public class AzureOpenAiCompletionService : ICompletionService
{
    private readonly ChatClient _client;

    /// <summary>
    /// Initializes a new instance of <see cref="AzureOpenAiCompletionService"/> using the provided settings.
    /// </summary>
    /// <param name="options">The Azure OpenAI configuration options.</param>
    public AzureOpenAiCompletionService(IOptions<AzureOpenAiSettings> options)
    {
        var settings = options.Value;
        var azureClient = new AzureOpenAIClient(
            new Uri(settings.Endpoint),
            new AzureKeyCredential(settings.ApiKey));
        _client = azureClient.GetChatClient(settings.DeploymentChat);
    }

    /// <summary>
    /// Sends a system and user message to the Azure OpenAI chat API and returns the generated text.
    /// </summary>
    /// <param name="systemPrompt">The system-level instruction that defines the model's behaviour.</param>
    /// <param name="userPrompt">The user message containing the question and document context.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The first text content item from the model's response.</returns>
    public async Task<string> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken)
    {
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        };

        var result = await _client.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        return result.Value.Content[0].Text;
    }
}

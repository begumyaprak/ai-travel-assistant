namespace AiTravelAssistant.Application.Interfaces.Services;

/// <summary>
/// Provides chat completion capabilities backed by a large language model.
/// </summary>
public interface ICompletionService
{
    /// <summary>
    /// Generates a completion for the given system and user prompts.
    /// </summary>
    /// <param name="systemPrompt">The system-level instruction that defines the model's behaviour.</param>
    /// <param name="userPrompt">The user-facing message containing the question and context.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The generated text response from the model.</returns>
    Task<string> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken);
}

namespace AiTravelAssistant.Infrastructure.Settings;

/// <summary>
/// Strongly-typed configuration settings for the Azure OpenAI service.
/// </summary>
public class AzureOpenAiSettings
{
    /// <summary>Gets or sets the Azure OpenAI resource endpoint URL.</summary>
    public string Endpoint { get; set; } = default!;

    /// <summary>Gets or sets the API key used to authenticate with Azure OpenAI.</summary>
    public string ApiKey { get; set; } = default!;

    /// <summary>Gets or sets the deployment name for the embedding model (e.g. "text-embedding-3-small").</summary>
    public string DeploymentEmbedding { get; set; } = default!;

    /// <summary>Gets or sets the deployment name for the chat completion model (e.g. "gpt-4o").</summary>
    public string DeploymentChat { get; set; } = default!;
}

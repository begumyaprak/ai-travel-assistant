namespace AiTravelAssistant.Infrastructure.Settings;

/// <summary>
/// Strongly-typed configuration settings for the Azure AI Search service.
/// </summary>
public class AzureSearchSettings
{
    /// <summary>Gets or sets the Azure AI Search service endpoint URL.</summary>
    public string Endpoint { get; set; } = default!;

    /// <summary>Gets or sets the API key used to authenticate with Azure AI Search.</summary>
    public string ApiKey { get; set; } = default!;

    /// <summary>Gets or sets the name of the search index that stores travel document chunks.</summary>
    public string IndexName { get; set; } = default!;

    /// <summary>Gets or sets the optional semantic configuration name; when set, semantic re-ranking is enabled.</summary>
    public string? SemanticConfigurationName { get; set; }
}

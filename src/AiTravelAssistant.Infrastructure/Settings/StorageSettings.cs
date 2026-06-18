namespace AiTravelAssistant.Infrastructure.Settings;

/// <summary>
/// Strongly-typed configuration settings for the local file storage provider.
/// </summary>
public class StorageSettings
{
    /// <summary>Gets or sets the base directory path where uploaded files are stored.</summary>
    public string BasePath { get; set; } = "./uploads";
}

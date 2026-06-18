namespace AiTravelAssistant.Application.Interfaces.Services;

/// <summary>
/// Extracts plain text from a document file stream, dispatching to the appropriate parser based on file type.
/// </summary>
public interface IDocumentParserService
{
    /// <summary>
    /// Parses the provided file stream and returns its full text content.
    /// </summary>
    /// <param name="content">A readable stream containing the raw file bytes.</param>
    /// <param name="fileName">The file name, used to determine the appropriate parser by extension.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The extracted plain-text content of the document.</returns>
    Task<string> ParseAsync(Stream content, string fileName, CancellationToken cancellationToken);
}

using DocumentFormat.OpenXml.Packaging;

namespace AiTravelAssistant.Infrastructure.Services;

/// <summary>
/// Extracts plain text from DOCX files using the Open XML SDK.
/// </summary>
public class DocxDocumentParser
{
    /// <summary>
    /// Reads the provided DOCX stream and returns the inner text of the main document body.
    /// </summary>
    /// <param name="content">A readable stream containing the raw DOCX bytes.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests (not used for this synchronous operation).</param>
    /// <returns>The plain-text content extracted from the document body.</returns>
    public Task<string> ParseAsync(Stream content, CancellationToken cancellationToken)
    {
        using var document = WordprocessingDocument.Open(content, isEditable: false);
        var text = document.MainDocumentPart?.Document?.Body?.InnerText ?? string.Empty;
        return Task.FromResult(text);
    }
}

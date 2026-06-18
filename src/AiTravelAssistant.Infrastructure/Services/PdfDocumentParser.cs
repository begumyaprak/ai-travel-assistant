using UglyToad.PdfPig;

namespace AiTravelAssistant.Infrastructure.Services;

/// <summary>
/// Extracts plain text from PDF files using the PdfPig library.
/// </summary>
public class PdfDocumentParser
{
    /// <summary>
    /// Reads all pages of the provided PDF stream and concatenates their word text.
    /// </summary>
    /// <param name="content">A readable stream containing the raw PDF bytes.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests (not used for this synchronous operation).</param>
    /// <returns>The plain-text content of all pages joined by newlines.</returns>
    public Task<string> ParseAsync(Stream content, CancellationToken cancellationToken)
    {
        using var document = PdfDocument.Open(content);
        var text = string.Join(
            Environment.NewLine,
            document.GetPages().Select(p => string.Join(" ", p.GetWords().Select(w => w.Text))));
        return Task.FromResult(text);
    }
}

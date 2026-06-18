using AiTravelAssistant.Application.Interfaces.Services;

namespace AiTravelAssistant.Infrastructure.Services;

/// <summary>
/// Implements <see cref="IDocumentParserService"/> by dispatching to the appropriate parser based on the file extension.
/// </summary>
public class DocumentParserService : IDocumentParserService
{
    private readonly PdfDocumentParser _pdfParser;
    private readonly DocxDocumentParser _docxParser;

    /// <summary>
    /// Initializes a new instance of <see cref="DocumentParserService"/>.
    /// </summary>
    /// <param name="pdfParser">The parser used for PDF files.</param>
    /// <param name="docxParser">The parser used for DOCX files.</param>
    public DocumentParserService(PdfDocumentParser pdfParser, DocxDocumentParser docxParser)
    {
        _pdfParser = pdfParser;
        _docxParser = docxParser;
    }

    /// <summary>
    /// Extracts plain text from the provided file stream by selecting the parser for the given file extension.
    /// </summary>
    /// <param name="content">A readable stream containing the raw file bytes.</param>
    /// <param name="fileName">The file name used to determine the parser by extension.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The extracted plain-text content of the document.</returns>
    /// <exception cref="NotSupportedException">Thrown when the file extension is not <c>.pdf</c> or <c>.docx</c>.</exception>
    public Task<string> ParseAsync(Stream content, string fileName, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => _pdfParser.ParseAsync(content, cancellationToken),
            ".docx" => _docxParser.ParseAsync(content, cancellationToken),
            _ => throw new NotSupportedException($"File type '{extension}' is not supported.")
        };
    }
}

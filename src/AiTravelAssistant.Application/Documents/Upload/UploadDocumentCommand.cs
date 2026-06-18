using AiTravelAssistant.Application.Common;
using MediatR;

namespace AiTravelAssistant.Application.Documents.Upload;

/// <summary>
/// Command to upload a new travel document, persist the file, and enqueue background processing.
/// </summary>
public class UploadDocumentCommand : IRequest<Result<Guid>>
{
    /// <summary>Gets the readable stream containing the raw file bytes.</summary>
    public required Stream FileStream { get; init; }

    /// <summary>Gets the original file name including extension (e.g. "paris-guide.pdf").</summary>
    public required string FileName { get; init; }

    /// <summary>Gets the category that classifies the document (e.g. "HotelPolicy", "DestinationGuide").</summary>
    public required string Category { get; init; }

    /// <summary>Gets the optional destination this document is associated with (e.g. "Barcelona").</summary>
    public string? Destination { get; init; }

    /// <summary>Gets the BCP-47 language code of the document content (e.g. "en", "tr").</summary>
    public required string Language { get; init; }

    /// <summary>Gets the identifier of the user or system submitting the upload.</summary>
    public required string UploadedBy { get; init; }
}

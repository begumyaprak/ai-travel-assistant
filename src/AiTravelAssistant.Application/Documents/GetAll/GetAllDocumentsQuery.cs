using AiTravelAssistant.Application.Common;
using AiTravelAssistant.Domain.Enums;
using MediatR;

namespace AiTravelAssistant.Application.Documents.GetAll;

/// <summary>
/// Query that returns a summary list of all uploaded documents.
/// </summary>
public record GetAllDocumentsQuery : IRequest<Result<IReadOnlyList<DocumentSummaryResponse>>>;

/// <summary>
/// A lightweight summary of a document returned by <see cref="GetAllDocumentsQuery"/>.
/// </summary>
/// <param name="Id">The unique identifier of the document.</param>
/// <param name="FileName">The original file name.</param>
/// <param name="Category">The category the document belongs to.</param>
/// <param name="Destination">The optional destination the document is associated with.</param>
/// <param name="Language">The BCP-47 language code of the document.</param>
/// <param name="Status">The current processing status of the document.</param>
/// <param name="UploadedAt">The UTC timestamp when the document was uploaded.</param>
/// <param name="UploadedBy">The identifier of the uploader.</param>
public record DocumentSummaryResponse(
    Guid Id,
    string FileName,
    string Category,
    string? Destination,
    string Language,
    DocumentStatus Status,
    DateTime UploadedAt,
    string UploadedBy);

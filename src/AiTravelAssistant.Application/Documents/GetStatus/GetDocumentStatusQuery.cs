using AiTravelAssistant.Application.Common;
using AiTravelAssistant.Domain.Enums;
using MediatR;

namespace AiTravelAssistant.Application.Documents.GetStatus;

/// <summary>
/// Query that returns the current processing status of a single document.
/// </summary>
/// <param name="DocumentId">The identifier of the document to retrieve status for.</param>
public record GetDocumentStatusQuery(Guid DocumentId) : IRequest<Result<DocumentStatusResponse>>;

/// <summary>
/// Contains the status and basic metadata of a document returned by <see cref="GetDocumentStatusQuery"/>.
/// </summary>
/// <param name="Id">The unique identifier of the document.</param>
/// <param name="FileName">The original file name.</param>
/// <param name="Status">The current processing status of the document.</param>
/// <param name="FailureReason">The reason for the last processing failure, or <see langword="null"/> if not failed.</param>
/// <param name="UploadedAt">The UTC timestamp when the document was uploaded.</param>
public record DocumentStatusResponse(
    Guid Id,
    string FileName,
    DocumentStatus Status,
    string? FailureReason,
    DateTime UploadedAt);

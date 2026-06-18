using AiTravelAssistant.Application.Common;
using AiTravelAssistant.Application.Common.Errors;
using AiTravelAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace AiTravelAssistant.Application.Documents.GetStatus;

/// <summary>
/// Handles <see cref="GetDocumentStatusQuery"/> by looking up the document and returning its status details.
/// </summary>
public class GetDocumentStatusQueryHandler : IRequestHandler<GetDocumentStatusQuery, Result<DocumentStatusResponse>>
{
    private readonly IDocumentRepository _documentRepository;

    /// <summary>
    /// Initializes a new instance of <see cref="GetDocumentStatusQueryHandler"/>.
    /// </summary>
    /// <param name="documentRepository">The repository used to retrieve the document record.</param>
    public GetDocumentStatusQueryHandler(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    /// <summary>
    /// Returns the processing status of the requested document, or a not-found failure if it does not exist.
    /// </summary>
    /// <param name="request">The query containing the document identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A result containing <see cref="DocumentStatusResponse"/> on success, or a failure with <c>DOCUMENT_NOT_FOUND</c>.</returns>
    public async Task<Result<DocumentStatusResponse>> Handle(GetDocumentStatusQuery request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);

        if (document is null)
            return Result<DocumentStatusResponse>.Failure(
                ApplicationErrors.Document.NotFound,
                $"Document with id '{request.DocumentId}' was not found.");

        return Result<DocumentStatusResponse>.Success(new DocumentStatusResponse(
            document.Id,
            document.FileName,
            document.Status,
            document.FailureReason,
            document.UploadedAt));
    }
}

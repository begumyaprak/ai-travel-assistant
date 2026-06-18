using AiTravelAssistant.Application.Common;
using AiTravelAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace AiTravelAssistant.Application.Documents.GetAll;

/// <summary>
/// Handles <see cref="GetAllDocumentsQuery"/> by retrieving all documents and projecting them to summary responses.
/// </summary>
public class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, Result<IReadOnlyList<DocumentSummaryResponse>>>
{
    private readonly IDocumentRepository _documentRepository;

    /// <summary>
    /// Initializes a new instance of <see cref="GetAllDocumentsQueryHandler"/>.
    /// </summary>
    /// <param name="documentRepository">The repository used to retrieve document records.</param>
    public GetAllDocumentsQueryHandler(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    /// <summary>
    /// Retrieves all documents and maps them to <see cref="DocumentSummaryResponse"/> items.
    /// </summary>
    /// <param name="request">The query (carries no parameters).</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A successful result containing the list of document summaries.</returns>
    public async Task<Result<IReadOnlyList<DocumentSummaryResponse>>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
    {
        var documents = await _documentRepository.GetAllAsync(cancellationToken);

        var response = documents
            .Select(d => new DocumentSummaryResponse(
                d.Id,
                d.FileName,
                d.Category,
                d.Destination,
                d.Language,
                d.Status,
                d.UploadedAt,
                d.UploadedBy))
            .ToList();

        return Result<IReadOnlyList<DocumentSummaryResponse>>.Success(response);
    }
}

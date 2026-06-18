using AiTravelAssistant.Application.Common;
using AiTravelAssistant.Application.Interfaces.Repositories;
using AiTravelAssistant.Application.Interfaces.Services;
using AiTravelAssistant.Domain.Entities;
using MediatR;

namespace AiTravelAssistant.Application.Documents.Upload;

/// <summary>
/// Handles <see cref="UploadDocumentCommand"/> by saving the file, creating a domain record, and scheduling processing.
/// </summary>
public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Result<Guid>>
{
    private readonly IDocumentStorageService _storageService;
    private readonly IDocumentRepository _documentRepository;
    private readonly IJobScheduler _jobScheduler;

    /// <summary>
    /// Initializes a new instance of <see cref="UploadDocumentCommandHandler"/>.
    /// </summary>
    /// <param name="storageService">The service used to persist the raw file.</param>
    /// <param name="documentRepository">The repository used to store the document record.</param>
    /// <param name="jobScheduler">The scheduler used to enqueue the background processing job.</param>
    public UploadDocumentCommandHandler(
        IDocumentStorageService storageService,
        IDocumentRepository documentRepository,
        IJobScheduler jobScheduler)
    {
        _storageService = storageService;
        _documentRepository = documentRepository;
        _jobScheduler = jobScheduler;
    }

    /// <summary>
    /// Executes the upload workflow and returns the new document's identifier.
    /// </summary>
    /// <param name="request">The upload command containing file content and metadata.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A successful result containing the new document's <see cref="Guid"/>.</returns>
    public async Task<Result<Guid>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var storagePath = await _storageService.SaveAsync(request.FileStream, request.FileName, cancellationToken);

        var document = Document.Create(
            request.FileName,
            request.Category,
            request.Destination,
            request.Language,
            request.UploadedBy,
            storagePath);

        await _documentRepository.AddAsync(document, cancellationToken);

        _jobScheduler.EnqueueDocumentProcessing(document.Id);

        return Result<Guid>.Success(document.Id);
    }
}

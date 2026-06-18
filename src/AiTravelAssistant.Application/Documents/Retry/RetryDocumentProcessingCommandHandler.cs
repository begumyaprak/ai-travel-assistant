using AiTravelAssistant.Application.Common;
using AiTravelAssistant.Application.Common.Errors;
using AiTravelAssistant.Application.Interfaces.Repositories;
using AiTravelAssistant.Application.Interfaces.Services;
using AiTravelAssistant.Domain.Enums;
using MediatR;

namespace AiTravelAssistant.Application.Documents.Retry;

/// <summary>
/// Handles <see cref="RetryDocumentProcessingCommand"/> by resetting the document state and scheduling a new job.
/// </summary>
public class RetryDocumentProcessingCommandHandler : IRequestHandler<RetryDocumentProcessingCommand, Result<bool>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IJobScheduler _jobScheduler;

    /// <summary>
    /// Initializes a new instance of <see cref="RetryDocumentProcessingCommandHandler"/>.
    /// </summary>
    /// <param name="documentRepository">The repository used to retrieve and update the document.</param>
    /// <param name="jobScheduler">The scheduler used to re-enqueue the processing job.</param>
    public RetryDocumentProcessingCommandHandler(
        IDocumentRepository documentRepository,
        IJobScheduler jobScheduler)
    {
        _documentRepository = documentRepository;
        _jobScheduler = jobScheduler;
    }

    /// <summary>
    /// Resets the document status and enqueues a new processing job, returning a failure if the document is not found or already processing.
    /// </summary>
    /// <param name="request">The command containing the document identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A successful result with <see langword="true"/> on success, or a failure result with an appropriate error code.</returns>
    public async Task<Result<bool>> Handle(RetryDocumentProcessingCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);

        if (document is null)
            return Result<bool>.Failure(
                ApplicationErrors.Document.NotFound,
                $"Document with id '{request.DocumentId}' was not found.");

        if (document.Status == DocumentStatus.Processing)
            return Result<bool>.Failure(
                ApplicationErrors.Document.AlreadyProcessing,
                $"Document '{request.DocumentId}' is currently being processed.");

        document.ResetForRetry();
        await _documentRepository.UpdateAsync(document, cancellationToken);

        _jobScheduler.EnqueueDocumentProcessing(document.Id);

        return Result<bool>.Success(true);
    }
}

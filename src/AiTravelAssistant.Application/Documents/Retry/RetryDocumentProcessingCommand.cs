using AiTravelAssistant.Application.Common;
using MediatR;

namespace AiTravelAssistant.Application.Documents.Retry;

/// <summary>
/// Command to reset a failed document back to the uploaded state and re-enqueue it for processing.
/// </summary>
/// <param name="DocumentId">The identifier of the document to retry.</param>
public record RetryDocumentProcessingCommand(Guid DocumentId) : IRequest<Result<bool>>;

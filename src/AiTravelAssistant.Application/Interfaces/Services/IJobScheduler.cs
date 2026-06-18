namespace AiTravelAssistant.Application.Interfaces.Services;

/// <summary>
/// Abstracts background job scheduling so the application layer does not depend on a specific job framework.
/// </summary>
public interface IJobScheduler
{
    /// <summary>
    /// Enqueues a document processing job for the specified document.
    /// </summary>
    /// <param name="documentId">The identifier of the document to process.</param>
    void EnqueueDocumentProcessing(Guid documentId);
}

using AiTravelAssistant.Application.Interfaces.Services;
using AiTravelAssistant.Infrastructure.Jobs;
using Hangfire;

namespace AiTravelAssistant.Infrastructure.Services;

/// <summary>
/// Implements <see cref="IJobScheduler"/> by enqueuing background jobs through Hangfire.
/// </summary>
public class HangfireJobScheduler : IJobScheduler
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    /// <summary>
    /// Initializes a new instance of <see cref="HangfireJobScheduler"/>.
    /// </summary>
    /// <param name="backgroundJobClient">The Hangfire client used to enqueue background jobs.</param>
    public HangfireJobScheduler(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    /// <summary>
    /// Enqueues a <see cref="DocumentProcessingJob"/> for the specified document.
    /// </summary>
    /// <param name="documentId">The identifier of the document to process.</param>
    public void EnqueueDocumentProcessing(Guid documentId)
    {
        _backgroundJobClient.Enqueue<DocumentProcessingJob>(job => job.ProcessAsync(documentId));
    }
}

using AiTravelAssistant.Domain.Common;
using AiTravelAssistant.Domain.Enums;
using AiTravelAssistant.Domain.Exceptions;

namespace AiTravelAssistant.Domain.Entities;

/// <summary>
/// Aggregate root representing an uploaded travel document and its processing lifecycle.
/// </summary>
public class Document : BaseEntity
{
    /// <summary>Gets the original file name of the uploaded document.</summary>
    public string FileName { get; private set; } = default!;

    /// <summary>Gets the category of the document (e.g. "HotelPolicy", "DestinationGuide").</summary>
    public string Category { get; private set; } = default!;

    /// <summary>Gets the optional destination this document is associated with (e.g. "Barcelona").</summary>
    public string? Destination { get; private set; }

    /// <summary>Gets the BCP-47 language code of the document content (e.g. "en", "tr").</summary>
    public string Language { get; private set; } = default!;

    /// <summary>Gets the current processing status of the document.</summary>
    public DocumentStatus Status { get; private set; }

    /// <summary>Gets the reason for the last processing failure, or <see langword="null"/> if the document has not failed.</summary>
    public string? FailureReason { get; private set; }

    /// <summary>Gets the UTC timestamp when the document was uploaded.</summary>
    public DateTime UploadedAt { get; private set; }

    /// <summary>Gets the identifier of the user or system that uploaded the document.</summary>
    public string UploadedBy { get; private set; } = default!;

    /// <summary>Gets the storage path where the raw file is persisted.</summary>
    public string StoragePath { get; private set; } = default!;

    private Document() { }

    /// <summary>
    /// Creates a new <see cref="Document"/> in the <see cref="DocumentStatus.Uploaded"/> state.
    /// </summary>
    /// <param name="fileName">The original file name.</param>
    /// <param name="category">The document category.</param>
    /// <param name="destination">The optional destination the document relates to.</param>
    /// <param name="language">The BCP-47 language code of the content.</param>
    /// <param name="uploadedBy">The identifier of the uploader.</param>
    /// <param name="storagePath">The path where the file has been stored.</param>
    /// <returns>A newly created <see cref="Document"/> instance.</returns>
    public static Document Create(
        string fileName,
        string category,
        string? destination,
        string language,
        string uploadedBy,
        string storagePath)
    {
        var now = DateTime.UtcNow;
        return new Document
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            Category = category,
            Destination = destination,
            Language = language,
            UploadedBy = uploadedBy,
            StoragePath = storagePath,
            Status = DocumentStatus.Uploaded,
            UploadedAt = now,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    /// <summary>
    /// Transitions the document status to <see cref="DocumentStatus.Processing"/>.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the document is already in the processing state.</exception>
    public void MarkAsProcessing()
    {
        if (Status == DocumentStatus.Processing)
            throw new DomainException("DOCUMENT_ALREADY_PROCESSING", $"Document '{Id}' is already being processed.");

        Status = DocumentStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Transitions the document status to <see cref="DocumentStatus.Processed"/> and clears any failure reason.
    /// </summary>
    public void MarkAsProcessed()
    {
        Status = DocumentStatus.Processed;
        FailureReason = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Transitions the document status to <see cref="DocumentStatus.Failed"/> and records the failure reason.
    /// </summary>
    /// <param name="reason">A description of why processing failed.</param>
    public void MarkAsFailed(string reason)
    {
        Status = DocumentStatus.Failed;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Resets the document back to <see cref="DocumentStatus.Uploaded"/> so it can be re-queued for processing.
    /// </summary>
    public void ResetForRetry()
    {
        Status = DocumentStatus.Uploaded;
        FailureReason = null;
        UpdatedAt = DateTime.UtcNow;
    }
}

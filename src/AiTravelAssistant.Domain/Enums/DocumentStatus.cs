namespace AiTravelAssistant.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of a document within the processing pipeline.
/// </summary>
public enum DocumentStatus
{
    /// <summary>The document has been uploaded and is awaiting processing.</summary>
    Uploaded,

    /// <summary>The document is currently being processed by a background job.</summary>
    Processing,

    /// <summary>The document has been successfully parsed, chunked, and indexed.</summary>
    Processed,

    /// <summary>Document processing failed; see <c>FailureReason</c> for details.</summary>
    Failed
}

namespace AiTravelAssistant.Application.Common.Errors;

/// <summary>
/// Provides well-known error code constants used across the application layer.
/// </summary>
public static class ApplicationErrors
{
    /// <summary>Error codes related to document operations.</summary>
    public static class Document
    {
        /// <summary>The requested document does not exist.</summary>
        public const string NotFound = "DOCUMENT_NOT_FOUND";

        /// <summary>The document is currently being processed and cannot be re-queued.</summary>
        public const string AlreadyProcessing = "DOCUMENT_ALREADY_PROCESSING";

        /// <summary>The background processing job for the document failed.</summary>
        public const string ProcessingFailed = "DOCUMENT_PROCESSING_FAILED";

        /// <summary>The uploaded file has an extension that is not supported.</summary>
        public const string UnsupportedFileType = "UNSUPPORTED_FILE_TYPE";

        /// <summary>The uploaded file exceeds the maximum permitted size.</summary>
        public const string FileTooLarge = "FILE_TOO_LARGE";
    }

    /// <summary>Error codes related to question-answering operations.</summary>
    public static class QA
    {
        /// <summary>The submitted question is empty or whitespace.</summary>
        public const string QuestionEmpty = "QUESTION_EMPTY";

        /// <summary>No processed documents are available to search against.</summary>
        public const string NoDocumentsAvailable = "NO_DOCUMENTS_AVAILABLE";

        /// <summary>No answer could be grounded in the available documents.</summary>
        public const string AnswerNotFound = "ANSWER_NOT_FOUND_IN_DOCUMENTS";
    }
}

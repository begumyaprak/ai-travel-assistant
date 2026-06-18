namespace AiTravelAssistant.Domain.Exceptions;

/// <summary>
/// Thrown when a requested document cannot be found in the system.
/// </summary>
public class DocumentNotFoundException : DomainException
{
    /// <summary>
    /// Initializes a new instance of <see cref="DocumentNotFoundException"/> for the specified document identifier.
    /// </summary>
    /// <param name="documentId">The identifier of the document that was not found.</param>
    public DocumentNotFoundException(Guid documentId)
        : base("DOCUMENT_NOT_FOUND", $"Document with id '{documentId}' was not found.")
    {
    }
}

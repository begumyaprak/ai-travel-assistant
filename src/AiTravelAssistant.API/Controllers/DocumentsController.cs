using AiTravelAssistant.API.Filters;
using AiTravelAssistant.Application.Documents.GetAll;
using AiTravelAssistant.Application.Documents.GetStatus;
using AiTravelAssistant.Application.Documents.Retry;
using AiTravelAssistant.Application.Documents.Upload;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AiTravelAssistant.API.Controllers;

/// <summary>
/// Exposes endpoints for uploading, listing, inspecting, and retrying travel documents.
/// </summary>
[Route("api/documents")]
public class DocumentsController : BaseApiController
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of <see cref="DocumentsController"/>.
    /// </summary>
    /// <param name="sender">The MediatR sender used to dispatch commands and queries.</param>
    public DocumentsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Uploads a travel document, persists it, and enqueues background processing.
    /// </summary>
    /// <param name="file">The file to upload (.pdf or .docx, max 10 MB).</param>
    /// <param name="category">The category that classifies the document.</param>
    /// <param name="destination">An optional destination the document is associated with.</param>
    /// <param name="language">The BCP-47 language code of the document content.</param>
    /// <param name="uploadedBy">The identifier of the user performing the upload.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The new document's identifier wrapped in the standard API envelope.</returns>
    [HttpPost("upload")]
    [ServiceFilter(typeof(ValidateFileFilter))]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromForm] string category,
        [FromForm] string? destination,
        [FromForm] string language,
        [FromForm] string uploadedBy,
        CancellationToken cancellationToken)
    {
        var command = new UploadDocumentCommand
        {
            FileStream = file.OpenReadStream(),
            FileName = file.FileName,
            Category = category,
            Destination = destination,
            Language = language,
            UploadedBy = uploadedBy
        };

        var result = await _sender.Send(command, cancellationToken);
        return ToResponse(result);
    }

    /// <summary>
    /// Returns a summary list of all uploaded documents ordered by upload date descending.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A list of document summaries wrapped in the standard API envelope.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllDocumentsQuery(), cancellationToken);
        return ToResponse(result);
    }

    /// <summary>
    /// Returns the current processing status of the specified document.
    /// </summary>
    /// <param name="id">The document identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The document status response, or 404 if the document does not exist.</returns>
    [HttpGet("{id:guid}/status")]
    public async Task<IActionResult> GetStatus(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetDocumentStatusQuery(id), cancellationToken);
        return ToResponse(result);
    }

    /// <summary>
    /// Resets a failed document and re-enqueues it for background processing.
    /// </summary>
    /// <param name="id">The document identifier to retry.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A success envelope, or 400/404 with an appropriate error code.</returns>
    [HttpPost("{id:guid}/retry")]
    public async Task<IActionResult> Retry(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RetryDocumentProcessingCommand(id), cancellationToken);
        return ToResponse(result);
    }
}

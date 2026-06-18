using AiTravelAssistant.Application.QA.AskQuestion;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AiTravelAssistant.API.Controllers;

/// <summary>
/// Exposes the question-answering endpoint for querying the travel document knowledge base.
/// </summary>
[Route("api/qa")]
public class QAController : BaseApiController
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of <see cref="QAController"/>.
    /// </summary>
    /// <param name="sender">The MediatR sender used to dispatch queries.</param>
    public QAController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Accepts a natural language question and returns a document-grounded answer with sources.
    /// </summary>
    /// <param name="request">The request body containing the question and optional filters.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A grounded answer with source references and confidence level, wrapped in the standard API envelope.</returns>
    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AskQuestionRequest request, CancellationToken cancellationToken)
    {
        var query = new AskQuestionQuery(request.Question, request.Destination, request.Category);
        var result = await _sender.Send(query, cancellationToken);
        return ToResponse(result);
    }
}

/// <summary>
/// Request body for the <c>POST /api/qa/ask</c> endpoint.
/// </summary>
/// <param name="Question">The natural language question to answer.</param>
/// <param name="Destination">An optional destination filter to narrow the document search scope.</param>
/// <param name="Category">An optional category filter to narrow the document search scope.</param>
public record AskQuestionRequest(string Question, string? Destination, string? Category);

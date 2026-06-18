using AiTravelAssistant.Application.Common;
using MediatR;

namespace AiTravelAssistant.Application.QA.AskQuestion;

/// <summary>
/// Query that submits a natural language question and returns a grounded answer from the document index.
/// </summary>
/// <param name="Question">The user's question in natural language.</param>
/// <param name="Destination">An optional destination filter to restrict the search scope.</param>
/// <param name="Category">An optional document category filter to restrict the search scope.</param>
public record AskQuestionQuery(
    string Question,
    string? Destination,
    string? Category) : IRequest<Result<AskQuestionResponse>>;

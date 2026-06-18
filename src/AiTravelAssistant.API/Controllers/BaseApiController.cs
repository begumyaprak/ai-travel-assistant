using AiTravelAssistant.API.Models;
using AiTravelAssistant.Application.Common;
using AiTravelAssistant.Application.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace AiTravelAssistant.API.Controllers;

/// <summary>
/// Base controller that provides shared helpers for converting <see cref="Result{T}"/> instances to HTTP responses.
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>Gets the trace identifier propagated by <c>TraceIdMiddleware</c> for the current request.</summary>
    protected string? TraceId => HttpContext.Items["TraceId"] as string;

    /// <summary>
    /// Maps a <see cref="Result{T}"/> to an appropriate <see cref="IActionResult"/>, returning 200, 400, or 404.
    /// </summary>
    /// <typeparam name="T">The payload type of the result.</typeparam>
    /// <param name="result">The application result to convert.</param>
    /// <returns>
    /// A 200 OK response with the data envelope on success; a 404 Not Found or 400 Bad Request on failure.
    /// </returns>
    protected IActionResult ToResponse<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(ApiResponse<T>.Ok(result.Data!, TraceId));

        var statusCode = result.ErrorCode == ApplicationErrors.Document.NotFound ? 404 : 400;
        return StatusCode(statusCode, ApiResponse<T?>.Fail(result.ErrorCode!, result.ErrorMessage!, TraceId));
    }
}

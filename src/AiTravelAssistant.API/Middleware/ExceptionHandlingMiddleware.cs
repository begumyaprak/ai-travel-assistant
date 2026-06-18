using System.Net;
using System.Text.Json;
using AiTravelAssistant.API.Models;
using AiTravelAssistant.Domain.Exceptions;
using FluentValidation;

namespace AiTravelAssistant.API.Middleware;

/// <summary>
/// Middleware that catches unhandled exceptions and converts them to structured JSON error responses.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ExceptionHandlingMiddleware"/>.
    /// </summary>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
    /// <param name="logger">The logger used to emit error events for unhandled exceptions.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the next middleware and handles any thrown exceptions by writing a consistent error response.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var firstError = ex.Errors.FirstOrDefault();
            await WriteErrorAsync(context, HttpStatusCode.BadRequest,
                "VALIDATION_ERROR",
                firstError?.ErrorMessage ?? "One or more validation errors occurred.");
        }
        catch (DomainException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, ex.Code, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, HttpStatusCode.InternalServerError,
                "INTERNAL_ERROR",
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode statusCode, string code, string message)
    {
        var traceId = context.Items["TraceId"] as string;
        var response = ApiResponse<object?>.Fail(code, message, traceId);

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}

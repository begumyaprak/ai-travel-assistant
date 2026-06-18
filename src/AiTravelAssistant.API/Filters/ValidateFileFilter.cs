using AiTravelAssistant.API.Models;
using AiTravelAssistant.Application.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AiTravelAssistant.API.Filters;

/// <summary>
/// Action filter that validates uploaded files before the controller action runs, enforcing size and extension rules.
/// </summary>
public class ValidateFileFilter : IAsyncActionFilter
{
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = [".pdf", ".docx"];

    /// <summary>
    /// Inspects the <see cref="IFormFile"/> argument in the action context and short-circuits with a 400 response if validation fails.
    /// </summary>
    /// <param name="context">The executing action context providing access to the request arguments and HTTP context.</param>
    /// <param name="next">The delegate to invoke when validation passes.</param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var file = context.ActionArguments.Values.OfType<IFormFile>().FirstOrDefault();
        var traceId = context.HttpContext.Items["TraceId"] as string;

        if (file is null)
        {
            context.Result = new BadRequestObjectResult(
                ApiResponse<object?>.Fail("FILE_REQUIRED", "A file must be provided.", traceId));
            return;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            context.Result = new BadRequestObjectResult(
                ApiResponse<object?>.Fail(
                    ApplicationErrors.Document.UnsupportedFileType,
                    "Only .pdf and .docx files are supported.",
                    traceId));
            return;
        }

        if (file.Length > MaxFileSizeBytes)
        {
            context.Result = new BadRequestObjectResult(
                ApiResponse<object?>.Fail(
                    ApplicationErrors.Document.FileTooLarge,
                    "File size must not exceed 10MB.",
                    traceId));
            return;
        }

        await next();
    }
}

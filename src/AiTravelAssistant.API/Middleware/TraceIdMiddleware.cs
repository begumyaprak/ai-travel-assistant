namespace AiTravelAssistant.API.Middleware;

/// <summary>
/// Middleware that propagates or generates a trace identifier for each request and exposes it via response headers.
/// </summary>
/// <remarks>
/// The trace ID is read from the <c>X-Trace-Id</c> request header if present; otherwise a new GUID is generated.
/// The resolved trace ID is stored in <c>HttpContext.Items["TraceId"]</c> so that other middleware and controllers can reference it.
/// </remarks>
public class TraceIdMiddleware
{
    private const string TraceIdHeader = "X-Trace-Id";
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of <see cref="TraceIdMiddleware"/>.
    /// </summary>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
    public TraceIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Resolves the trace ID, stores it in the context, appends it to the response header, and calls the next middleware.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = context.Request.Headers[TraceIdHeader].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Items["TraceId"] = traceId;
        context.Response.Headers[TraceIdHeader] = traceId;

        await _next(context);
    }
}

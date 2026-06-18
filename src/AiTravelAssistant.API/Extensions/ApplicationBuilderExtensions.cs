using AiTravelAssistant.API.Middleware;
using Hangfire;

namespace AiTravelAssistant.API.Extensions;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/> that register API middleware and optional dashboard routes.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Registers the <see cref="TraceIdMiddleware"/> and <see cref="ExceptionHandlingMiddleware"/> in the pipeline.
    /// </summary>
    /// <param name="app">The application builder to configure.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> for chaining.</returns>
    public static IApplicationBuilder UseApiMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<TraceIdMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }

    /// <summary>
    /// Mounts the Hangfire Dashboard at <c>/hangfire</c> when the application is running in the Development environment.
    /// </summary>
    /// <param name="app">The application builder to configure.</param>
    /// <param name="env">The hosting environment used to determine whether the dashboard should be exposed.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> for chaining.</returns>
    public static IApplicationBuilder UseHangfireDashboardIfEnabled(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseHangfireDashboard("/hangfire");

        return app;
    }
}

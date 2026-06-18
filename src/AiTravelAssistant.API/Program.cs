using AiTravelAssistant.API.Extensions;
using AiTravelAssistant.API.Filters;
using AiTravelAssistant.Application.Common.Behaviors;
using AiTravelAssistant.Application.Documents.Upload;
using AiTravelAssistant.Infrastructure.DependencyInjection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// ── Application layer ──────────────────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(UploadDocumentCommand).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(UploadDocumentCommand).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// ── Infrastructure layer ───────────────────────────────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);

// ── CORS ───────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueClient", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ── API layer ──────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddScoped<ValidateFileFilter>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── Health checks ──────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "postgresql")
    .AddRedis(builder.Configuration["Redis:ConnectionString"]!, name: "redis");

// ──────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseApiMiddleware();
app.UseCors("AllowVueClient");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Travel Assistant API v1"));
}

app.UseHangfireDashboardIfEnabled(app.Environment);
app.UseHttpsRedirection();
app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        });
    }
});

app.Run();

public partial class Program { }

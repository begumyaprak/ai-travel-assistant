using AiTravelAssistant.Application.Interfaces.Repositories;
using AiTravelAssistant.Application.Interfaces.Services;
using AiTravelAssistant.Infrastructure.Jobs;
using AiTravelAssistant.Infrastructure.Persistence;
using AiTravelAssistant.Infrastructure.Persistence.Repositories;
using AiTravelAssistant.Infrastructure.Services;
using AiTravelAssistant.Infrastructure.Settings;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace AiTravelAssistant.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering all infrastructure-layer services with the DI container.
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Registers EF Core, Redis, Azure OpenAI, Azure AI Search, Hangfire, repositories, and storage services.
    /// </summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <param name="configuration">The application configuration used to resolve connection strings and settings.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<AzureOpenAiSettings>(configuration.GetSection("AzureOpenAi"));
        services.Configure<AzureSearchSettings>(configuration.GetSection("AzureSearch"));
        services.Configure<RedisSettings>(configuration.GetSection("Redis"));
        services.Configure<StorageSettings>(configuration.GetSection("Storage"));

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]!));

        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IDocumentChunkRepository, DocumentChunkRepository>();

        services.AddScoped<IEmbeddingService, AzureOpenAiEmbeddingService>();
        services.AddScoped<ICompletionService, AzureOpenAiCompletionService>();
        services.AddScoped<ISearchService, AzureAiSearchService>();
        services.AddScoped<ICacheService, RedisResponseCacheService>();
        services.AddScoped<IDocumentParserService, DocumentParserService>();
        services.AddScoped<IDocumentStorageService, LocalFileStorageService>();
        services.AddScoped<IJobScheduler, HangfireJobScheduler>();

        services.AddScoped<PdfDocumentParser>();
        services.AddScoped<DocxDocumentParser>();
        services.AddScoped<DocumentProcessingJob>();

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(c =>
                c.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"))));

        services.AddHangfireServer();

        return services;
    }
}

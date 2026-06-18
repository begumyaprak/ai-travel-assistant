namespace AiTravelAssistant.Infrastructure.Settings;

/// <summary>
/// Strongly-typed configuration settings for the Redis cache connection.
/// </summary>
public class RedisSettings
{
    /// <summary>Gets or sets the StackExchange.Redis connection string (e.g. "localhost:6379").</summary>
    public string ConnectionString { get; set; } = default!;
}

using System.Text.Json;
using AiTravelAssistant.Application.Interfaces.Services;
using StackExchange.Redis;

namespace AiTravelAssistant.Infrastructure.Services;

/// <summary>
/// Implements <see cref="ICacheService"/> using Redis via StackExchange.Redis with JSON serialization.
/// </summary>
public class RedisResponseCacheService : ICacheService
{
    private readonly IDatabase _db;

    /// <summary>
    /// Initializes a new instance of <see cref="RedisResponseCacheService"/> and obtains the default Redis database.
    /// </summary>
    /// <param name="redis">The multiplexer used to access the Redis connection.</param>
    public RedisResponseCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    /// <summary>
    /// Retrieves a JSON-serialized value from Redis and deserializes it to <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type of the cached value.</typeparam>
    /// <param name="key">The Redis key to look up.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests (not used by the Redis client).</param>
    /// <returns>The deserialized value, or <see langword="default"/> if the key does not exist.</returns>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty) return default;
        return JsonSerializer.Deserialize<T>(value!);
    }

    /// <summary>
    /// Serializes <paramref name="value"/> to JSON and stores it in Redis under the given key.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The Redis key under which to store the value.</param>
    /// <param name="value">The value to serialize and cache.</param>
    /// <param name="ttl">The optional expiry duration; the entry does not expire when <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests (not used by the Redis client).</param>
    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(value);
        if (ttl.HasValue)
            await _db.StringSetAsync(key, json, ttl.Value);
        else
            await _db.StringSetAsync(key, json);
    }

    /// <summary>
    /// Deletes the Redis key and its associated value.
    /// </summary>
    /// <param name="key">The Redis key to delete.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests (not used by the Redis client).</param>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        await _db.KeyDeleteAsync(key);
    }
}

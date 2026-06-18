namespace AiTravelAssistant.Application.Interfaces.Services;

/// <summary>
/// Provides generic cache get, set, and remove operations for response caching.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Retrieves a cached value by key, or <see langword="default"/> if the key does not exist.
    /// </summary>
    /// <typeparam name="T">The expected type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The deserialized cached value, or <see langword="default"/> if not found.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);

    /// <summary>
    /// Stores a value in the cache under the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="ttl">The optional time-to-live for the cache entry; no expiry is set when <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    Task SetAsync<T>(string key, T value, TimeSpan? ttl, CancellationToken cancellationToken);

    /// <summary>
    /// Removes the cache entry associated with the specified key.
    /// </summary>
    /// <param name="key">The cache key to evict.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken);
}

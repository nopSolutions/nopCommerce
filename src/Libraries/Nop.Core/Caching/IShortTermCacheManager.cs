namespace Nop.Core.Caching;

/// <summary>
/// Represents a manager for caching during an HTTP request (short term caching)
/// </summary>
public partial interface IShortTermCacheManager : ICacheKeyService
{
    /// <summary>
    /// Remove items by cache key prefix
    /// </summary>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    void RemoveByPrefix(string prefix, params object[] prefixParameters);

    /// <summary>
    /// Remove the value with the specified key from the cache
    /// </summary>
    /// <param name="cacheKey">Cache key</param>
    /// <param name="cacheKeyParameters">Parameters to create cache key</param>
    void Remove(string cacheKey, params object[] cacheKeyParameters);

    /// <summary>
    /// Get a cached item. If it's not in the cache yet, then load and cache it
    /// </summary>
    /// <typeparam name="T">Type of cached item</typeparam>
    /// /// <param name="acquire">Function to load item if it's not in the cache yet</param>
    /// <param name="cacheKey">Initial cache key</param>
    /// <param name="cacheKeyParameters">Parameters to create cache key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cached value associated with the specified key
    /// </returns>
    Task<T> GetAsync<T>(Func<Task<T>> acquire, CacheKey cacheKey, params object[] cacheKeyParameters);
}
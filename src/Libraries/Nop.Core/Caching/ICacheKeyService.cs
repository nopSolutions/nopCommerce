namespace Nop.Core.Caching;

/// <summary>
/// Cache key service interface
/// </summary>
public partial interface ICacheKeyService
{
    /// <summary>
    /// Create a copy of cache key and fills it by passed parameters
    /// </summary>
    /// <param name="cacheKey">Initial cache key</param>
    /// <param name="cacheKeyParameters">Parameters to create cache key</param>
    /// <returns>Cache key</returns>
    CacheKey PrepareKey(CacheKey cacheKey, params object[] cacheKeyParameters);

    /// <summary>
    /// Create a copy of cache key using the default cache time and fills it by passed parameters
    /// </summary>
    /// <param name="cacheKey">Initial cache key</param>
    /// <param name="cacheKeyParameters">Parameters to create cache key</param>
    /// <returns>Cache key</returns>
    CacheKey PrepareKeyForDefaultCache(CacheKey cacheKey, params object[] cacheKeyParameters);
}
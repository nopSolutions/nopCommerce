using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Nop.Core.Caching;

/// <summary>
/// Represents a per request cache manager
/// </summary>
public partial class PerRequestCacheManager : CacheKeyService, IShortTermCacheManager
{
    #region Fields

    protected readonly ConcurrentTrie<object> _concurrentCollection;

    #endregion

    #region Ctor

    public PerRequestCacheManager(AppSettings appSettings) : base(appSettings)
    {
        _concurrentCollection = new ConcurrentTrie<object>();
    }

    #endregion

    #region Methods
        
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
    public async Task<T> GetAsync<T>(Func<Task<T>> acquire, CacheKey cacheKey, params object[] cacheKeyParameters)
    {
        var key = cacheKey.Create(CreateCacheKeyParameters, cacheKeyParameters).Key;

        if (_concurrentCollection.TryGetValue(key, out var data))
            return (T)data;

        var result = await acquire();

        if (result != null)
            _concurrentCollection.Add(key, result);

        return result;
    }
        
    /// <summary>
    /// Remove items by cache key prefix
    /// </summary>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    public virtual void RemoveByPrefix(string prefix, params object[] prefixParameters)
    {
        var keyPrefix = PrepareKeyPrefix(prefix, prefixParameters);
        _concurrentCollection.Prune(keyPrefix, out _);
    }
        
    /// <summary>
    /// Remove the value with the specified key from the cache
    /// </summary>
    /// <param name="cacheKey">Cache key</param>
    /// <param name="cacheKeyParameters">Parameters to create cache key</param>
    public virtual void Remove(string cacheKey, params object[] cacheKeyParameters)
    {
        _concurrentCollection.Remove(PrepareKey(new CacheKey(cacheKey), cacheKeyParameters).Key);
    }

    #endregion
}
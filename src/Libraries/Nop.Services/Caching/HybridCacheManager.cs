using System.Collections.Concurrent;
using HybridRedisCache;
using Microsoft.Extensions.Logging;
using Nop.Core.Caching;
using Nop.Core.Configuration;

namespace Nop.Services.Caching;
/// <summary>
/// Represents a manager for hybrid caching - local cache with a Redis store (http://redis.io/) backplane.
/// Mostly it'll be used when running in a web farm, AWS or Azure. But of course it can be also used on any server or environment
/// </summary>
public class HybridCacheManager : CacheKeyService, IStaticCacheManager
{
    #region Fields
    // Flag: Has Dispose already been called?
    protected bool _disposed;
    private readonly IHybridCache _cache;

    /// <summary>
    /// Holds the keys known by this nopCommerce instance
    /// </summary>
    private readonly ILogger<HybridCacheManager> _logger;

    //private readonly ILogger _logger;
    protected static CancellationTokenSource _clearToken = new();

    protected readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _timerCancellationTokenSource;
    private readonly Task _timerTask; protected readonly ConcurrentDictionary<string, bool> _prefixesToRemove = new();
    #endregion

    public HybridCacheManager(AppSettings appSettings,
                                 IHybridCache cache,
                                 ILogger<HybridCacheManager> logger) : base(appSettings)
    {
        _cache = cache;
        _logger = logger;
        var publishIntervalMs = appSettings.Get<DistributedCacheConfig>().PublishIntervalMs;
        if (publishIntervalMs > 0)
        {
            var timeSpan = TimeSpan.FromMilliseconds(publishIntervalMs);
            _timer = new(timeSpan);
            _timerCancellationTokenSource = new CancellationTokenSource();

            // Start the background timer task
            _timerTask = RunPeriodicTimerAsync(_timerCancellationTokenSource.Token);
        }
    }

    #region Utilities
    private async Task RunPeriodicTimerAsync(CancellationToken token)
    {//this avoids cache stampede by batching the removal of items from the cache
        try
        {
            while (await _timer.WaitForNextTickAsync(token))
            {
                await RemoveQueuedPrefixesAsync(token);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"HybridCacheManager: Error in periodic timer task: {ex.Message}");
        }
    }
    private async ValueTask<bool> RemoveQueuedPrefixesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var prefixesToRemove = _prefixesToRemove.Keys.ToList();
            _prefixesToRemove.Clear();  //clear the list of items we are about to remove, so new events can create a new list for next time

            foreach (var prefix in prefixesToRemove)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _cache.RemoveWithPatternAsync(prefix, flags: Flags.None, token: cancellationToken);
            }

            return true;
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
            // Re-throw cancellation exceptions to allow proper cleanup
            throw;
        }
        catch (Exception ex)
        {
            //log the error
            _logger?.LogError(ex, $"HybridCacheManager: Error removing queued prefixes: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Remove the value with the specified key from the cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="removeFromInstance">Remove from instance</param>
    protected virtual async Task RemoveAsync(string key, bool removeFromInstance = true)
    {
        await _cache.RemoveAsync(key);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Remove the value with the specified key from the cache
    /// </summary>
    /// <param name="cacheKey">Cache key</param>
    /// <param name="cacheKeyParameters">Parameters to create cache key</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters)
    {
        await RemoveAsync(PrepareKey(cacheKey, cacheKeyParameters).Key);
    }

    /// <summary>
    /// Get a cached item. If it's not in the cache yet, then load and cache it
    /// </summary>
    /// <typeparam name="T">Type of cached item</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="acquire">Function to load item if it's not in the cache yet</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cached value associated with the specified key
    /// </returns>
    public async Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire)
    {
        if ((key?.CacheTime ?? 0) <= 0)
        {//don't cache
            return await acquire();
        }
        return await _cache.GetAsync(key.Key,
            async (key) => await acquire(),
            TimeSpan.FromMinutes(key.CacheTime),
            TimeSpan.FromMinutes(key.CacheTime),
            Flags.None);

    }

    public async Task<T> GetAsync<T>(CacheKey key, T defaultValue = default)
    {
        return await _cache.GetAsync(key.Key,
             _ => Task.FromResult(defaultValue));
    }

    /// <summary>
    /// Get a cached item. If it's not in the cache yet, then load and cache it
    /// </summary>
    /// <typeparam name="T">Type of cached item</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="acquire">Function to load item if it's not in the cache yet</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cached value associated with the specified key
    /// </returns>
    public Task<T> GetAsync<T>(CacheKey key, Func<T> acquire)
    {
        return GetAsync(key, () => Task.FromResult(acquire()));
    }

    /// <summary>
    /// Get a cached item as an <see cref="object"/> instance, or null on a cache miss.
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cached value associated with the specified key, or null if none was found
    /// </returns>
    public async Task<object> GetAsync(CacheKey key)
    {
        return await GetAsync<object>(key, () => null);
    }

    /// <summary>
    /// Add the specified key and object to the cache
    /// </summary>
    /// <param name="key">Key of cached item</param>
    /// <param name="data">Value for caching</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SetAsync<T>(CacheKey key, T data)
    {
        if (data == null || (key?.CacheTime ?? 0) <= 0)
            return;

        await _cache.SetAsync(key.Key,
            data,
            TimeSpan.FromMinutes(key.CacheTime),
            TimeSpan.FromMinutes(key.CacheTime),
            Flags.None);
    }

    /// <summary>
    /// Remove items by cache key prefix
    /// </summary>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
    {
        prefix = PrepareKeyPrefix(prefix, prefixParameters);

        //queue up the prefix to be removed from the cache - this avoids cache stampede
        _prefixesToRemove.TryAdd(prefix, true);
    }

    /// <summary>
    /// Clear all cache data
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ClearAsync()
    {
        await _cache.ClearAllAsync(Flags.PreferMaster);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // don't dispose of the MemoryCache, as it is injected
            // Cancel the timer and wait for completion
            _timerCancellationTokenSource?.Cancel();

            try
            {
                _timerTask?.Wait(TimeSpan.FromSeconds(5)); // Wait up to 5 seconds
            }
            catch (AggregateException)
            {
                // Timer task was cancelled, which is expected
            }

            _timer?.Dispose();
            _timerCancellationTokenSource?.Dispose();
            _clearToken.Dispose();
        }
        _disposed = true;
    }

    #endregion

}

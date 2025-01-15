using System.Collections.Concurrent;
using System.Data;
using System.Net;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Hybrid;
using Newtonsoft.Json;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using StackExchange.Redis;

namespace Nop.Services.Caching;

/// <summary>
/// Represents a memory cache manager 
/// </summary>
/// <remarks>
/// This class should be registered on IoC as singleton instance
/// </remarks>
public partial class StaticCacheManager : CacheKeyService, IStaticCacheManager
{
    #region Fields

    /// <summary>
    /// Holds the keys known by this nopCommerce instance
    /// </summary>
    protected readonly ICacheKeyManager _localKeyManager;
    protected readonly HybridCache _hybridCache;
    protected readonly IConcurrentCollection<object> _concurrentCollection;
    private readonly DistributedCacheConfig _distributedCacheConfig;

    protected static CancellationTokenSource _clearToken = new();

    /// <summary>
    /// Holds ongoing acquisition tasks, used to avoid duplicating work
    /// </summary>
    protected readonly ConcurrentDictionary<string, Lazy<Task<object>>> _ongoing = new();

    protected const string NULL_VALUE = "nopCommerce_null_value";

    #endregion

    #region Ctor

    public StaticCacheManager(AppSettings appSettings,
        HybridCache hybridCache,
        ICacheKeyManager cacheKeyManager,
        IConcurrentCollection<object> concurrentCollection)
        : base(appSettings)
    {
        _hybridCache = hybridCache;
        _localKeyManager = cacheKeyManager;
        _concurrentCollection = concurrentCollection;
        _distributedCacheConfig = appSettings.Get<DistributedCacheConfig>();
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Clear all data on this instance
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual void ClearInstanceData()
    {
        _concurrentCollection.Clear();
        _localKeyManager.Clear();
    }

    /// <summary>
    /// Remove items by cache key prefix
    /// </summary>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    /// <returns>The removed keys</returns>
    protected virtual IEnumerable<string> RemoveByPrefixInstanceData(string prefix, params object[] prefixParameters)
    {
        var keyPrefix = PrepareKeyPrefix(prefix, prefixParameters);
        _concurrentCollection.Prune(keyPrefix, out _);

        return _localKeyManager.RemoveByPrefix(keyPrefix);
    }

    /// <summary>
    /// Prepare cache entry options for the passed key
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns>Cache entry options</returns>
    protected virtual HybridCacheEntryOptions PrepareEntryOptions(CacheKey key)
    {
        //set expiration time for the passed cache key
        return new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(key.CacheTime),
            LocalCacheExpiration = TimeSpan.FromMinutes(key.CacheTime)
        };
    }

    /// <summary>
    /// Add the specified key and object to the local cache
    /// </summary>
    /// <param name="key">Key of cached item</param>
    /// <param name="value">Value for caching</param>
    protected virtual void SetLocal(string key, object value)
    {
        _concurrentCollection.Add(key, value);
        _localKeyManager.AddKey(key);
    }

    /// <summary>
    /// Remove the value with the specified key from the cache
    /// </summary>
    /// <param name="key">Cache key</param>
    protected virtual void RemoveLocal(string key)
    {
        _concurrentCollection.Remove(key);
        _localKeyManager.RemoveKey(key);
    }

    /// <summary>
    /// Try get a cached item. If it's not in the cache yet, then return default object
    /// </summary>
    /// <typeparam name="T">Type of cached item</typeparam>
    /// <param name="key">Cache key</param>
    protected virtual async Task<(bool isSet, T item)> TryGetItemAsync<T>(string key)
    {
        var cachedValue = await _hybridCache.GetOrCreateAsync(key, _ => new ValueTask<string>(NULL_VALUE));

        var isSet = true;

        T rez;

        if (cachedValue.Equals(NULL_VALUE, StringComparison.InvariantCulture))
        {
            await _hybridCache.RemoveAsync(key);
            isSet = false;
            rez = default;
        }
        else
            rez = JsonConvert.DeserializeObject<T>(cachedValue);

        return (isSet, rez);
    }
    
    /// <summary>
    /// Remove the value with the specified key from the cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="removeFromInstance">Remove from instance</param>
    protected virtual async Task RemoveAsync(string key, bool removeFromInstance = true)
    {
        _ongoing.TryRemove(key, out _);
        await _hybridCache.RemoveAsync(key);

        if (!removeFromInstance)
            return;

        RemoveLocal(key);
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
        if (_concurrentCollection.TryGetValue(key.Key, out var data))
            return (T)data;

        var lazy = _ongoing.GetOrAdd(key.Key, _ => new(async () => await acquire(), true));
        var setTask = Task.CompletedTask;

        try
        {
            if (lazy.IsValueCreated)
                return (T)await lazy.Value;

            var (isSet, item) = await TryGetItemAsync<T>(key.Key);
            if (!isSet)
            {
                item = (T)await lazy.Value;

                if (key.CacheTime == 0 || item == null)
                    return item;

                setTask = _hybridCache.SetAsync(key.Key, JsonConvert.SerializeObject(item), PrepareEntryOptions(key), cancellationToken: _clearToken.Token).AsTask();
            }

            SetLocal(key.Key, item);

            return item;
        }
        finally
        {
            _ = setTask.ContinueWith(_ => _ongoing.TryRemove(new KeyValuePair<string, Lazy<Task<object>>>(key.Key, lazy)));
        }
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
    public async Task<T> GetAsync<T>(CacheKey key, Func<T> acquire)
    {
        return await GetAsync(key, () => Task.FromResult(acquire()));
    }

    /// <summary>
    /// Get a cached item. If it's not in the cache yet, return a default value
    /// </summary>
    /// <typeparam name="T">Type of cached item</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="defaultValue">A default value to return if the key is not present in the cache</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cached value associated with the specified key, or the default value if none was found
    /// </returns>
    public async Task<T> GetAsync<T>(CacheKey key, T defaultValue = default)
    {
        return await GetAsync(key, () => Task.FromResult(defaultValue));
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
        return await GetAsync<object>(key);
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

        var lazy = new Lazy<Task<object>>(() => Task.FromResult(data as object), true);

        try
        {
            _ongoing.TryAdd(key.Key, lazy);
            // await the lazy task in order to force value creation instead of directly setting data
            // this way, other cache manager instances can access it while it is being set
            SetLocal(key.Key, await lazy.Value);
            await _hybridCache.SetAsync(key.Key, JsonConvert.SerializeObject(data), PrepareEntryOptions(key), cancellationToken: _clearToken.Token);
        }
        finally
        {
            _ongoing.TryRemove(new KeyValuePair<string, Lazy<Task<object>>>(key.Key, lazy));
        }
    }

    /// <summary>
    /// Clear all cache data
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ClearAsync()
    {
        await _clearToken.CancelAsync();
        _clearToken.Dispose();
        _clearToken = new CancellationTokenSource();

        ClearInstanceData();
    }

    /// <summary>
    /// Remove items by cache key prefix
    /// </summary>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
    {
        async Task remove()
        {
            var keyPrefix = PrepareKeyPrefix(prefix, prefixParameters);

            foreach (var key in RemoveByPrefixInstanceData(keyPrefix))
                await RemoveAsync(key, false);
        }

        if (_distributedCacheConfig.Enabled)
        {
            switch (_distributedCacheConfig.DistributedCacheType)
            {
                case DistributedCacheType.SqlServer:

                    async Task performAction(SqlCommand command, params SqlParameter[] parameters)
                    {
                        var conn = new SqlConnection(_distributedCacheConfig.ConnectionString);

                        try
                        {
                            await conn.OpenAsync();
                            command.Connection = conn;
                            if (parameters.Any())
                                command.Parameters.AddRange(parameters);

                            await command.ExecuteNonQueryAsync();
                        }
                        finally
                        {
                            await conn.CloseAsync();
                        }
                    }

                    prefix = PrepareKeyPrefix(prefix, prefixParameters);

                    var command = new SqlCommand($"DELETE FROM {_distributedCacheConfig.SchemaName}.{_distributedCacheConfig.TableName} WHERE Id LIKE @Prefix + '%'");

                    await performAction(command, new SqlParameter("Prefix", SqlDbType.NVarChar) { Value = prefix });

                    RemoveByPrefixInstanceData(prefix);
                    break;
                case DistributedCacheType.Redis:

                    var connectionWrapper = EngineContext.Current.Resolve<IRedisConnectionWrapper>();

                    async Task<IEnumerable<RedisKey>> getKeys(EndPoint endPoint, string prefix = null)
                    {
                        return await(await connectionWrapper.GetServerAsync(endPoint))
                            .KeysAsync((await connectionWrapper.GetDatabaseAsync()).Database, string.IsNullOrEmpty(prefix) ? null : $"{prefix}*")
                            .ToListAsync();
                    }

                    prefix = PrepareKeyPrefix(prefix, prefixParameters);
                    var db = await connectionWrapper.GetDatabaseAsync();

                    var instanceName = _distributedCacheConfig.InstanceName ?? string.Empty;

                    foreach (var endPoint in await connectionWrapper.GetEndPointsAsync())
                    {
                        var keys = await getKeys(endPoint, instanceName + prefix);
                        db.KeyDelete(keys.ToArray());
                    }

                    RemoveByPrefixInstanceData(prefix);
                    break;
                case DistributedCacheType.Memory:
                case DistributedCacheType.RedisSynchronizedMemory:
                default:
                    await remove();
                    break;
            }

            return;
        }

        await remove();
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #endregion
}
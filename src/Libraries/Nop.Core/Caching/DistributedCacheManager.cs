using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Represents a base distributed cache 
    /// </summary>
    public abstract class DistributedCacheManager : CacheKeyService, IStaticCacheManager
    {
        #region Fields

        /// <summary>
        /// Holds the keys known by this nopCommerce instance
        /// </summary>
        protected readonly ICacheKeyManager _localKeyManager;
        protected readonly IDistributedCache _distributedCache;
        protected readonly ConcurrentTrie<object> _perRequestCache = new();

        /// <summary>
        /// Holds ongoing acquisition tasks, used to avoid duplicating work
        /// </summary>
        protected readonly ConcurrentDictionary<string, Lazy<Task<object>>> _ongoing = new();

        #endregion

        #region Ctor

        protected DistributedCacheManager(AppSettings appSettings,
            IDistributedCache distributedCache,
            ICacheKeyManager cacheKeyManager)
            : base(appSettings)
        {
            _distributedCache = distributedCache;
            _localKeyManager = cacheKeyManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Clear all data on this instance
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected void ClearInstanceData()
        {
            _perRequestCache.Clear();
            _localKeyManager.Clear();
        }

        /// <summary>
        /// Remove items by cache key prefix
        /// </summary>
        /// <param name="prefix">Cache key prefix</param>
        /// <param name="prefixParameters">Parameters to create cache key prefix</param>
        /// <returns>The removed keys</returns>
        protected IEnumerable<string> RemoveByPrefixInstanceData(string prefix, params object[] prefixParameters)
        {
            var keyPrefix = PrepareKeyPrefix(prefix, prefixParameters);
            _perRequestCache.Prune(keyPrefix, out _);

            return _localKeyManager.RemoveByPrefix(keyPrefix);
        }

        /// <summary>
        /// Prepare cache entry options for the passed key
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <returns>Cache entry options</returns>
        private static DistributedCacheEntryOptions PrepareEntryOptions(CacheKey key)
        {
            //set expiration time for the passed cache key
            return new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(key.CacheTime)
            };
        }

        /// <summary>
        /// Add the specified key and object to the local cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="value">Value for caching</param>
        protected void SetLocal(string key, object value)
        {
            _perRequestCache.Add(key, value);
            _localKeyManager.AddKey(key);
        }

        /// <summary>
        /// Remove the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Cache key</param>
        protected void RemoveLocal(string key)
        {
            _perRequestCache.Remove(key);
            _localKeyManager.RemoveKey(key);
        }

        /// <summary>
        /// Try get a cached item. If it's not in the cache yet, then return default object
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        protected async Task<(bool isSet, T item)> TryGetItemAsync<T>(string key)
        {
            var json = await _distributedCache.GetStringAsync(key);

            return string.IsNullOrEmpty(json)
              ? (false, default)
              : (true, item: JsonConvert.DeserializeObject<T>(json));
        }

        /// <summary>
        /// Remove the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="removeFromInstance">Remove from instance</param>
        protected async Task RemoveAsync(string key, bool removeFromInstance = true)
        {
            _ongoing.TryRemove(key, out _);
            await _distributedCache.RemoveAsync(key);

            if(!removeFromInstance)
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
            if (_perRequestCache.TryGetValue(key.Key, out var data))
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

                    setTask = _distributedCache.SetStringAsync(
                        key.Key,
                        JsonConvert.SerializeObject(item),
                        PrepareEntryOptions(key));
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
        public Task<T> GetAsync<T>(CacheKey key, Func<T> acquire)
        {
            return GetAsync(key, () => Task.FromResult(acquire()));
        }

        public async Task<T> GetAsync<T>(CacheKey key, T defaultValue = default)
        {
            var value = await _distributedCache.GetStringAsync(key.Key);

            return value != null
                ? JsonConvert.DeserializeObject<T>(value)
                : defaultValue;
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
                await _distributedCache.SetStringAsync(key.Key, JsonConvert.SerializeObject(data), PrepareEntryOptions(key));
            }
            finally
            {
                _ongoing.TryRemove(new KeyValuePair<string, Lazy<Task<object>>>(key.Key, lazy));
            }
        }

        /// <summary>
        /// Remove items by cache key prefix
        /// </summary>
        /// <param name="prefix">Cache key prefix</param>
        /// <param name="prefixParameters">Parameters to create cache key prefix</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public abstract Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters);

        /// <summary>
        /// Clear all cache data
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public abstract Task ClearAsync();

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

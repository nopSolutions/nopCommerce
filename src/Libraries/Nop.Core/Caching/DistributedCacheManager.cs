using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Nito.AsyncEx;
using Nop.Core.ComponentModel;
using Nop.Core.Configuration;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Represents a distributed cache 
    /// </summary>
    public partial class DistributedCacheManager: CacheKeyService, ILocker, IStaticCacheManager
    {
        #region Fields

        private readonly IDistributedCache _distributedCache;
        private readonly PerRequestCache _perRequestCache;
        private static readonly List<string> _keys;
        private static readonly AsyncLock _locker;

        #endregion

        #region Ctor

        static DistributedCacheManager()
        {
            _locker = new AsyncLock();
            _keys = new List<string>();
        }

        public DistributedCacheManager(AppSettings appSettings, IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor) :base(appSettings)
        {
            _distributedCache = distributedCache;
            _perRequestCache = new PerRequestCache(httpContextAccessor);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare cache entry options for the passed key
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <returns>Cache entry options</returns>
        private DistributedCacheEntryOptions PrepareEntryOptions(CacheKey key)
        {
            //set expiration time for the passed cache key
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(key.CacheTime)
            };
            
            return options;
        }

        /// <summary>
        /// Try to get the cached item
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the flag which indicate is the key exists in the cache, cached item or default value
        /// </returns>
        private async Task<(bool isSet, T item)> TryGetItemAsync<T>(CacheKey key)
        {
            var json = await _distributedCache.GetStringAsync(key.Key);

            if (string.IsNullOrEmpty(json)) 
                return (false, default);

            var item = JsonConvert.DeserializeObject<T>(json);
            _perRequestCache.Set(key.Key, item);

            return (true, item);
        }

        /// <summary>
        /// Try to get the cached item
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <returns>Flag which indicate is the key exists in the cache, cached item or default value</returns>
        private (bool isSet, T item) TryGetItem<T>(CacheKey key)
        {
            var json = _distributedCache.GetString(key.Key);

            if (string.IsNullOrEmpty(json))
                return (false, default);

            var item = JsonConvert.DeserializeObject<T>(json);
            _perRequestCache.Set(key.Key, item);

            return (true, item);
        }

        /// <summary>
        /// Add the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        private void Set(CacheKey key, object data)
        {
            if ((key?.CacheTime ?? 0) <= 0 || data == null)
                return;

            _distributedCache.SetString(key.Key, JsonConvert.SerializeObject(data), PrepareEntryOptions(key));
            _perRequestCache.Set(key.Key, data);

            using var _ = _locker.Lock();
            _keys.Add(key.Key);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
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
            //little performance workaround here:
            //we use "PerRequestCache" to cache a loaded object in memory for the current HTTP request.
            //this way we won't connect to Redis server many times per HTTP request (e.g. each time to load a locale or setting)
            if (_perRequestCache.IsSet(key.Key))
                return _perRequestCache.Get(key.Key, () => default(T));

            if (key.CacheTime <= 0)
                return await acquire();

            var (isSet, item) = await TryGetItemAsync<T>(key);

            if (isSet)
                return item;

            var result = await acquire();

            if (result != null)
                await SetAsync(key, result);

            return result;
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
            //little performance workaround here:
            //we use "PerRequestCache" to cache a loaded object in memory for the current HTTP request.
            //this way we won't connect to Redis server many times per HTTP request (e.g. each time to load a locale or setting)
            if (_perRequestCache.IsSet(key.Key))
                return _perRequestCache.Get(key.Key, () => default(T));

            if (key.CacheTime <= 0)
                return acquire();

            var (isSet, item) = await TryGetItemAsync<T>(key);

            if (isSet)
                return item;

            var result = acquire();

            if (result != null)
                await SetAsync(key, result);

            return result;
        }

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>The cached value associated with the specified key</returns>
        public T Get<T>(CacheKey key, Func<T> acquire)
        {
            //little performance workaround here:
            //we use "PerRequestCache" to cache a loaded object in memory for the current HTTP request.
            //this way we won't connect to Redis server many times per HTTP request (e.g. each time to load a locale or setting)
            if (_perRequestCache.IsSet(key.Key))
                return _perRequestCache.Get(key.Key, () => default(T));

            if (key.CacheTime <= 0)
                return acquire();

            var (isSet, item) = TryGetItem<T>(key);

            if (isSet)
                return item;

            var result = acquire();

            if (result != null)
                Set(key, result);

            return result;
        }

        /// <summary>
        /// Remove the value with the specified key from the cache
        /// </summary>
        /// <param name="cacheKey">Cache key</param>
        /// <param name="cacheKeyParameters">Parameters to create cache key</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            cacheKey = PrepareKey(cacheKey, cacheKeyParameters);

            await _distributedCache.RemoveAsync(cacheKey.Key);
            _perRequestCache.Remove(cacheKey.Key);

            using var _ = await _locker.LockAsync();
            _keys.Remove(cacheKey.Key);
        }

        /// <summary>
        /// Add the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task SetAsync(CacheKey key, object data)
        {
            if ((key?.CacheTime ?? 0) <= 0 || data == null)
                return;

            await _distributedCache.SetStringAsync(key.Key, JsonConvert.SerializeObject(data), PrepareEntryOptions(key));
            _perRequestCache.Set(key.Key, data);

            using var _ = await _locker.LockAsync();
            _keys.Add(key.Key);
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
            _perRequestCache.RemoveByPrefix(prefix);

            using var _ = await _locker.LockAsync();
            
            foreach (var key in _keys.Where(key => key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)).ToList())
            {
                await _distributedCache.RemoveAsync(key);
                _keys.Remove(key);
            }
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ClearAsync()
        {
            //we can't use _perRequestCache.Clear(),
            //because HttpContext stores some server data that we should not delete
            foreach (var redisKey in _keys)
                _perRequestCache.Remove(redisKey);

            using var _ = await _locker.LockAsync();

            foreach (var key in _keys) 
                await _distributedCache.RemoveAsync(key);

            _keys.Clear();
        }

        /// <summary>
        /// Perform some action with exclusive lock
        /// </summary>
        /// <param name="resource">The key we are locking on</param>
        /// <param name="expirationTime">The time after which the lock will automatically be expired</param>
        /// <param name="action">Action to be performed with locking</param>
        /// <returns>True if lock was acquired and action was performed; otherwise false</returns>
        public bool PerformActionWithLock(string resource, TimeSpan expirationTime, Action action)
        {
            //ensure that lock is acquired
            if (!string.IsNullOrEmpty(_distributedCache.GetString(resource)))
                return false;

            try
            {
                _distributedCache.SetString(resource, resource, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expirationTime
                });

                //perform action
                action();

                return true;
            }
            finally
            {
                //release lock even if action fails
                _distributedCache.Remove(resource);
            }
        }

        #endregion

        #region Nested class

        /// <summary>
        /// Represents a manager for caching during an HTTP request (short term caching)
        /// </summary>
        protected class PerRequestCache
        {
            #region Fields

            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly ReaderWriterLockSlim _lockSlim;

            #endregion

            #region Ctor

            public PerRequestCache(IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;

                _lockSlim = new ReaderWriterLockSlim();
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Get a key/value collection that can be used to share data within the scope of this request
            /// </summary>
            protected virtual IDictionary<object, object> GetItems()
            {
                return _httpContextAccessor.HttpContext?.Items;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Get a cached item. If it's not in the cache yet, then load and cache it
            /// </summary>
            /// <typeparam name="T">Type of cached item</typeparam>
            /// <param name="key">Cache key</param>
            /// <param name="acquire">Function to load item if it's not in the cache yet</param>
            /// <returns>The cached value associated with the specified key</returns>
            public virtual T Get<T>(string key, Func<T> acquire)
            {
                IDictionary<object, object> items;

                using (new ReaderWriteLockDisposable(_lockSlim, ReaderWriteLockType.Read))
                {
                    items = GetItems();
                    if (items == null)
                        return acquire();

                    //item already is in cache, so return it
                    if (items[key] != null)
                        return (T)items[key];
                }

                //or create it using passed function
                var result = acquire();

                //and set in cache (if cache time is defined)
                using (new ReaderWriteLockDisposable(_lockSlim))
                    items[key] = result;

                return result;
            }

            /// <summary>
            /// Add the specified key and object to the cache
            /// </summary>
            /// <param name="key">Key of cached item</param>
            /// <param name="data">Value for caching</param>
            public virtual void Set(string key, object data)
            {
                if (data == null)
                    return;

                using (new ReaderWriteLockDisposable(_lockSlim))
                {
                    var items = GetItems();
                    if (items == null)
                        return;

                    items[key] = data;
                }
            }

            /// <summary>
            /// Get a value indicating whether the value associated with the specified key is cached
            /// </summary>
            /// <param name="key">Key of cached item</param>
            /// <returns>True if item already is in cache; otherwise false</returns>
            public virtual bool IsSet(string key)
            {
                using (new ReaderWriteLockDisposable(_lockSlim, ReaderWriteLockType.Read))
                {
                    var items = GetItems();
                    return items?[key] != null;
                }
            }

            /// <summary>
            /// Remove the value with the specified key from the cache
            /// </summary>
            /// <param name="key">Key of cached item</param>
            public virtual void Remove(string key)
            {
                using (new ReaderWriteLockDisposable(_lockSlim))
                {
                    var items = GetItems();
                    items?.Remove(key);
                }
            }

            /// <summary>
            /// Remove items by key prefix
            /// </summary>
            /// <param name="prefix">String key prefix</param>
            public virtual void RemoveByPrefix(string prefix)
            {
                using (new ReaderWriteLockDisposable(_lockSlim, ReaderWriteLockType.UpgradeableRead))
                {
                    var items = GetItems();
                    if (items == null)
                        return;

                    //get cache keys that matches pattern
                    var regex = new Regex(prefix,
                        RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    var matchesKeys = items.Keys.Select(p => p.ToString())
                        .Where(key => regex.IsMatch(key ?? string.Empty)).ToList();

                    if (!matchesKeys.Any())
                        return;

                    using (new ReaderWriteLockDisposable(_lockSlim))
                    //remove matching values
                    foreach (var key in matchesKeys) 
                        items.Remove(key);
                }
            }

            #endregion
        }

        #endregion
    }
}

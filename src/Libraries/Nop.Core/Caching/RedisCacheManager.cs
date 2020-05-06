using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Nop.Core.Configuration;
using Nop.Core.Redis;
using Nop.Core.Security;
using StackExchange.Redis;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Represents a manager for caching in Redis store (http://redis.io/).
    /// Mostly it'll be used when running in a web farm or Azure. But of course it can be also used on any server or environment
    /// </summary>
    public partial class RedisCacheManager : IStaticCacheManager
    {
        #region Fields

        private bool _disposed;
        private readonly IRedisConnectionWrapper _connectionWrapper;
        private readonly IDatabase _db;
        private readonly MemoryCacheManager _memoryCacheManager;

        #endregion

        #region Ctor

        public RedisCacheManager(IRedisConnectionWrapper connectionWrapper,
            NopConfig config)
        {
            if (string.IsNullOrEmpty(config.RedisConnectionString))
                throw new Exception("Redis connection string is empty");

            // ConnectionMultiplexer.Connect should only be called once and shared between callers
            _connectionWrapper = connectionWrapper;

            _db = _connectionWrapper.GetDatabase(config.RedisDatabaseId ?? (int)RedisDatabaseNumber.Cache);

            _memoryCacheManager = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()));
        }

        ~RedisCacheManager()
        {
            _memoryCacheManager.Dispose();
        }

        #endregion
        
        #region Utilities

        /// <summary>
        /// Gets the list of cache keys prefix
        /// </summary>
        /// <param name="endPoint">Network address</param>
        /// <param name="prefix">String key pattern</param>
        /// <returns>List of cache keys</returns>
        protected virtual IEnumerable<RedisKey> GetKeys(EndPoint endPoint, string prefix = null)
        {
            var server = _connectionWrapper.GetServer(endPoint);

            //we can use the code below (commented), but it requires administration permission - ",allowAdmin=true"
            //server.FlushDatabase();

            var keys = server.Keys(_db.Database, string.IsNullOrEmpty(prefix) ? null : $"{prefix}*");

            //we should always persist the data protection key list
            keys = keys.Where(key => !key.ToString().Equals(NopDataProtectionDefaults.RedisDataProtectionKey,
                StringComparison.OrdinalIgnoreCase));

            return keys;
        }
        
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Key of cached item</param>
        /// <returns>The cached value associated with the specified key</returns>
        protected virtual async Task<T> GetAsync<T>(CacheKey key)
        {
            //little performance workaround here:
            //we use "MemoryCacheManager" to cache a loaded object in memory for the current HTTP request.
            //this way we won't connect to Redis server many times per HTTP request (e.g. each time to load a locale or setting)
            if (_memoryCacheManager.IsSet(key))
                return _memoryCacheManager.Get(key, () => default(T));

            //get serialized item from cache
            var serializedItem = await _db.StringGetAsync(key.Key);
            if (!serializedItem.HasValue)
                return default;

            //deserialize item
            var item = JsonConvert.DeserializeObject<T>(serializedItem);
            if (item == null)
                return default;

            //set item in the per-request cache
            _memoryCacheManager.Set(key, item);

            return item;
        }
        
        /// <summary>
        /// Adds the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        /// <param name="cacheTime">Cache time in minutes</param>
        protected virtual async Task SetAsync(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            //set cache time
            var expiresIn = TimeSpan.FromMinutes(cacheTime);

            //serialize item
            var serializedItem = JsonConvert.SerializeObject(data);

            //and set it to cache
            await _db.StringSetAsync(key, serializedItem, expiresIn);
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>True if item already is in cache; otherwise false</returns>
        protected virtual async Task<bool> IsSetAsync(CacheKey key)
        {
            //little performance workaround here:
            //we use "MemoryCacheManager" to cache a loaded object in memory for the current HTTP request.
            //this way we won't connect to Redis server many times per HTTP request (e.g. each time to load a locale or setting)
            if (_memoryCacheManager.IsSet(key))
                return true;

            return await _db.KeyExistsAsync(key.Key);
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
        public async Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire)
        {
            //item already is in cache, so return it
            if (await IsSetAsync(key))
                return await GetAsync<T>(key);

            //or create it using passed function
            var result = await acquire();

            //and set in cache (if cache time is defined)
            if (key.CacheTime > 0)
                await SetAsync(key.Key, result, key.CacheTime);

            return result;
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Key of cached item</param>
        /// <returns>The cached value associated with the specified key</returns>
        public virtual T Get<T>(CacheKey key)
        {
            //little performance workaround here:
            //we use "MemoryCacheManager" to cache a loaded object in memory for the current HTTP request.
            //this way we won't connect to Redis server many times per HTTP request (e.g. each time to load a locale or setting)
            if (_memoryCacheManager.IsSet(key))
                return _memoryCacheManager.Get(key, () => default(T));

            //get serialized item from cache
            var serializedItem = _db.StringGet(key.Key);
            if (!serializedItem.HasValue)
                return default;

            //deserialize item
            var item = JsonConvert.DeserializeObject<T>(serializedItem);
            if (item == null)
                return default;

            //set item in the per-request cache
            _memoryCacheManager.Set(key, item);

            return item;
        }

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>The cached value associated with the specified key</returns>
        public virtual T Get<T>(CacheKey key, Func<T> acquire)
        {
            //item already is in cache, so return it
            if (IsSet(key))
                return Get<T>(key);

            //or create it using passed function
            var result = acquire();

            //and set in cache (if cache time is defined)
            if (key.CacheTime > 0)
                Set(key, result);

            return result;
        }

        /// <summary>
        /// Adds the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        public virtual void Set(CacheKey key, object data)
        {
            if (data == null)
                return;

            //set cache time
            var expiresIn = TimeSpan.FromMinutes(key.CacheTime);

            //serialize item
            var serializedItem = JsonConvert.SerializeObject(data);

            //and set it to cache
            _db.StringSet(key.Key, serializedItem, expiresIn);
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>True if item already is in cache; otherwise false</returns>
        public virtual bool IsSet(CacheKey key)
        {
            //little performance workaround here:
            //we use "MemoryCacheManager" to cache a loaded object in memory for the current HTTP request.
            //this way we won't connect to Redis server many times per HTTP request (e.g. each time to load a locale or setting)
            if (_memoryCacheManager.IsSet(key))
                return true;

            return _db.KeyExists(key.Key);
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        public virtual void Remove(CacheKey key)
        {
            //we should always persist the data protection key list
            if (key.Key.Equals(NopDataProtectionDefaults.RedisDataProtectionKey, StringComparison.OrdinalIgnoreCase))
                return;

            //remove item from caches
            _db.KeyDelete(key.Key);
            _memoryCacheManager.Remove(key);
        }

        /// <summary>
        /// Removes items by key prefix
        /// </summary>
        /// <param name="prefix">String key prefix</param>
        public virtual void RemoveByPrefix(string prefix)
        {
            _memoryCacheManager.RemoveByPrefix(prefix);

            foreach (var endPoint in _connectionWrapper.GetEndPoints())
            {
                var keys = GetKeys(endPoint, prefix);

                _db.KeyDelete(keys.ToArray());
            }
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public virtual void Clear()
        {
            foreach (var endPoint in _connectionWrapper.GetEndPoints())
            {
                var keys = GetKeys(endPoint).ToArray();
                
                _memoryCacheManager.Clear();

                _db.KeyDelete(keys);
            }
        }

        /// <summary>
        /// Dispose cache manager
        /// </summary>
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

            _disposed = true;
        }

        #endregion
    }
}
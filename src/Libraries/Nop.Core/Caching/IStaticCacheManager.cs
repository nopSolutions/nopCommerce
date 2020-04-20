using System;
using System.Threading.Tasks;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Represents a manager for caching between HTTP requests (long term caching)
    /// </summary>
    public interface IStaticCacheManager : IDisposable
    {
        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>The cached value associated with the specified key</returns>
        T Get<T>(CacheKey key, Func<T> acquire);

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>The cached value associated with the specified key</returns>
        Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire);

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        void Remove(CacheKey key);

        /// <summary>
        /// Adds the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        void Set(CacheKey key, object data);

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>True if item already is in cache; otherwise false</returns>
        bool IsSet(CacheKey key);

        /// <summary>
        /// Removes items by key prefix
        /// </summary>
        /// <param name="prefix">String key prefix</param>
        void RemoveByPrefix(string prefix);

        /// <summary>
        /// Clear all cache data
        /// </summary>
        void Clear();
    }
}
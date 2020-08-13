using System;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Configuration;

namespace Nop.Tests
{
    /// <summary>
    /// Represents a null cache (caches nothing)
    /// </summary>
    public partial class TestCacheManager : CacheKeyService, IStaticCacheManager
    {
        private bool _disposed;

        public TestCacheManager() : base(new NopConfig())
        {

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
            return acquire();
        }
        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>The cached value associated with the specified key</returns>
        public async Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire)
        {
            var rez = await acquire();
            return rez;
        }

        public void Remove(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
        }

        /// <summary>
        /// Adds the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        public virtual void Set(CacheKey key, object data)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>True if item already is in cache; otherwise false</returns>
        public bool IsSet(CacheKey key)
        {
            return false;
        }

        public void RemoveByPrefix(string prefix, params object[] prefixParameters)
        {
        }
        
        /// <summary>
        /// Clear all cache data
        /// </summary>
        public virtual void Clear()
        {
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

            if (disposing)
            {
                //nothing special
            }

            _disposed = true;
        }
    }
}

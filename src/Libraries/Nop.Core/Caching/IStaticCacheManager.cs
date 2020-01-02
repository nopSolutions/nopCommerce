using System;
using System.Threading.Tasks;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Represents a manager for caching between HTTP requests (long term caching)
    /// </summary>
    public interface IStaticCacheManager : ICacheManager
    {
        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <param name="cacheTime">Cache time in minutes; pass 0 to do not cache; pass null to use the default time</param>
        /// <returns>The cached value associated with the specified key</returns>
        Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime = null);
    }
}
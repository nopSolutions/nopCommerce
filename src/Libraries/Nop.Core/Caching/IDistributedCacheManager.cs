using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Caching
{
    public interface IDistributedCacheManager : IStaticCacheManager
    {
        /// <summary>
        /// Removes the value with the specified key from the cache.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <param name="utcDateTime">The utc date time for the cache entry.</param>
        void Remove(string key, DateTime? utcDateTime);

        /// <summary>
        /// Removes items by key prefix.
        /// </summary>
        /// <param name="prefix">String key prefix.</param>
        /// <param name="utcDateTime">The utc date time for the cache entry.</param>
        void RemoveByPrefix(string prefix, DateTime? utcDateTime);

        /// <summary>
        /// Adds the specified key and object to the cache and optionally syncs it to the queue.
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        /// <param name="utcDateTime">The utc date time for the cache entry.</param>
        void Set(CacheKey key, object data, DateTime? utcDateTime);

        /// <summary>
        /// Cleans the memory cache and optionally synchronizes the operation on the queue.
        /// </summary>
        /// <param name="synchronize">Whether to synchronize to the queue</param>
        void Clear(bool synchronize);
    }
}

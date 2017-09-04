using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Extensions of ICacheManager
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// Get default cache time in minutes
        /// </summary>
        private static int DefaultCacheTimeMinutes { get { return 60; } }

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Cached item</returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            //use default cache time
            return Get(cacheManager, key, DefaultCacheTimeMinutes, acquire);
        }

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="key">Cache key</param>
        /// <param name="cacheTime">Cache time in minutes (0 - do not cache)</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Cached item</returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
        {
            //item already is in cache, so return it
            if (cacheManager.IsSet(key))
                return cacheManager.Get<T>(key);

            //or create it using passed function
            var result = acquire();

            //and set in cache (if cache time is defined)
            if (cacheTime > 0)
                cacheManager.Set(key, result, cacheTime);

            return result;
        }

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="pattern">Pattern</param>
        /// <param name="keys">All keys in the cache</param>
        public static void RemoveByPattern(this ICacheManager cacheManager, string pattern, IEnumerable<string> keys)
        {
            //get cache keys that matches pattern
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchesKeys = keys.Where(key => regex.IsMatch(key)).ToList();

            //remove matching values
            matchesKeys.ForEach(key => cacheManager.Remove(key));
        }

        /// <summary>
        /// Get original (base) entity. Throw an exception if it cannot be loaded
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Type</returns>
        public static Type GetOriginalEntityType(this IEntityForCaching entity)
        {
            var type = entity.GetType()?.BaseType;
            if (type == null)
                throw new Exception("Original entity type cannot be loaded");

            return type;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;

namespace Nop.Services.Caching.Extensions
{
    public static class IQueryableExtensions
    {
        private static IStaticCacheManager CacheManager => EngineContext.Current.Resolve<IStaticCacheManager>();

        /// <summary>
        /// Gets a cached list
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="query">Elements of source to put on cache</param>
        /// <param name="cacheKey">Cache key</param>
        /// <returns>Cached list</returns>
        public static IList<T> ToCachedList<T>(this IQueryable<T> query, CacheKey cacheKey)
        {
            return cacheKey == null ? query.ToList() : CacheManager.Get(cacheKey, query.ToList);
        }

        /// <summary>
        /// Gets a cached array
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="query">Elements of source to put on cache</param>
        /// <param name="cacheKey">Cache key</param>
        /// <returns>Cached array</returns>
        public static T[] ToCachedArray<T>(this IQueryable<T> query, CacheKey cacheKey)
        {
            return cacheKey == null ? query.ToArray() : CacheManager.Get(cacheKey, query.ToArray);
        }

        /// <summary>
        /// Gets a cached first element of a sequence, or a default value
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="query">Elements of source to put on cache</param>
        /// <param name="cacheKey">Cache key</param>
        /// <returns>Cached first element or default value</returns>
        public static T ToCachedFirstOrDefault<T>(this IQueryable<T> query, CacheKey cacheKey)
        {
            return cacheKey == null
                ? query.FirstOrDefault()
                : CacheManager.Get(cacheKey, query.FirstOrDefault);
        }

        /// <summary>
        /// Gets only element of a sequence, and throws an exception
        /// if there is not exactly one element in the sequence
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="query">Elements of source to put on cache</param>
        /// <param name="cacheKey">Cache key</param>
        /// <returns>Cached single element</returns>
        public static T ToCachedSingle<T>(this IQueryable<T> query, CacheKey cacheKey)
        {
            return cacheKey == null
                ? query.Single()
                : CacheManager.Get(cacheKey, query.Single);
        }

        /// <summary>
        /// Gets a cached value which determines whether a sequence contains any elements
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="query">Elements of source to put on cache</param>
        /// <param name="cacheKey">Cache key</param>
        /// <returns>Cached value which determines whether a sequence contains any elements</returns>
        public static bool ToCachedAny<T>(this IQueryable<T> query, CacheKey cacheKey)
        {
            return cacheKey == null
                ? query.Any()
                : CacheManager.Get(cacheKey, query.Any);
        }

        /// <summary>
        /// Gets a cached number of elements in a sequence
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="query">Elements of source to put on cache</param>
        /// <param name="cacheKey">Cache key</param>
        /// <returns>Cached number of elements</returns>
        public static int ToCachedCount<T>(this IQueryable<T> query, CacheKey cacheKey)
        {
            return cacheKey == null
                ? query.Count()
                : CacheManager.Get(cacheKey, query.Count);
        }
    }
}

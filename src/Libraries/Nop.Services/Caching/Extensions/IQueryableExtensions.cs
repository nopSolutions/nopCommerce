using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
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
        public static async Task<IList<T>> ToCachedList<T>(this IQueryable<T> query, CacheKey cacheKey)
        {
            return cacheKey == null ? await query.ToListAsync() : await CacheManager.Get(cacheKey, () => query.ToListAsync());
        }

        /// <summary>
        /// Gets a cached array
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="query">Elements of source to put on cache</param>
        /// <param name="cacheKey">Cache key</param>
        /// <returns>Cached array</returns>
        public static async Task<T[]> ToCachedArray<T>(this IQueryable<T> query, CacheKey cacheKey)
        {
            return cacheKey == null ? await query.ToArrayAsync() : await CacheManager.Get(cacheKey, () => query.ToArrayAsync());
        }

        /// <summary>
        /// Gets a cached first element of a sequence, or a default value
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="query">Elements of source to put on cache</param>
        /// <param name="cacheKey">Cache key</param>
        /// <returns>Cached first element or default value</returns>
        public static async Task<T> ToCachedFirstOrDefault<T>(this IQueryable<T> query, CacheKey cacheKey)
        {
            return cacheKey == null
                ? await query.FirstOrDefaultAsync()
                : await CacheManager.Get(cacheKey, () => query.FirstOrDefaultAsync());
        }

        /// <summary>
        /// Gets only element of a sequence, and throws an exception
        /// if there is not exactly one element in the sequence
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="query">Elements of source to put on cache</param>
        /// <param name="cacheKey">Cache key</param>
        /// <returns>Cached single element</returns>
        public static async Task<T> ToCachedSingle<T>(this IQueryable<T> query, CacheKey cacheKey)
        {
            return cacheKey == null
                ? await query.SingleAsync()
                : await CacheManager.Get(cacheKey, () => query.SingleAsync());
        }

        /// <summary>
        /// Gets a cached value which determines whether a sequence contains any elements
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="query">Elements of source to put on cache</param>
        /// <param name="cacheKey">Cache key</param>
        /// <returns>Cached value which determines whether a sequence contains any elements</returns>
        public static async Task<bool> ToCachedAny<T>(this IQueryable<T> query, CacheKey cacheKey)
        {
            return cacheKey == null
                ? await query.AnyAsync()
                : await CacheManager.Get(cacheKey, () => query.AnyAsync());
        }

        /// <summary>
        /// Gets a cached number of elements in a sequence
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="query">Elements of source to put on cache</param>
        /// <param name="cacheKey">Cache key</param>
        /// <returns>Cached number of elements</returns>
        public static async Task<int> ToCachedCount<T>(this IQueryable<T> query, CacheKey cacheKey)
        {
            return cacheKey == null
                ? await query.CountAsync()
                : await CacheManager.Get(cacheKey, () => query.CountAsync());
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;

namespace Nop.Services.Caching.Extensions
{
    public static class IQueryableExtensions
    {
        private static IStaticCacheManager CacheManager => EngineContext.Current.Resolve<IStaticCacheManager>();

        public static IList<T> ToCachedList<T>(this IQueryable<T> query, string cacheKey)
        {
            return string.IsNullOrEmpty(cacheKey) ? query.ToList() : CacheManager.Get(cacheKey, query.ToList);
        }

        public static T[] ToCachedArray<T>(this IQueryable<T> query, string cacheKey)
        {
            return string.IsNullOrEmpty(cacheKey) ? query.ToArray() : CacheManager.Get(cacheKey, query.ToArray);
        }

        public static T ToCachedFirstOrDefault<T>(this IQueryable<T> query, string cacheKey)
        {
            return string.IsNullOrEmpty(cacheKey)
                ? query.FirstOrDefault()
                : CacheManager.Get(cacheKey, query.FirstOrDefault);
        }

        public static T ToCachedSingle<T>(this IQueryable<T> query, string cacheKey)
        {
            return string.IsNullOrEmpty(cacheKey)
                ? query.Single()
                : CacheManager.Get(cacheKey, query.Single);
        }

        public static IPagedList<T> ToCachedPagedList<T>(this IQueryable<T> query, string cacheKey, int pageIndex,
            int pageSize)
        {
            return string.IsNullOrEmpty(cacheKey)
                ? new PagedList<T>(query, pageIndex, pageSize)
                : CacheManager.Get(cacheKey, () => new PagedList<T>(query, pageIndex, pageSize));
        }

        public static bool ToCachedAny<T>(this IQueryable<T> query, string cacheKey)
        {
            return string.IsNullOrEmpty(cacheKey)
                ? query.Any()
                : CacheManager.Get(cacheKey, query.Any);
        }

        public static int ToCachedCount<T>(this IQueryable<T> query, string cacheKey)
        {
            return string.IsNullOrEmpty(cacheKey)
                ? query.Count()
                : CacheManager.Get(cacheKey, query.Count);
        }
    }
}

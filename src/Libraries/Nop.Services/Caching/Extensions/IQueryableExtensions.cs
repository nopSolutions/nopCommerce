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

        public static IList<T> ToCachedList<T>(this IQueryable<T> query, string cacheKey = null)
        {
            return CacheManager.Get(cacheKey ?? query.ToString(), query.ToList);
        }

        public static T[] ToCachedArray<T>(this IQueryable<T> query, string cacheKey = null)
        {
            return CacheManager.Get(cacheKey ?? query.ToString(), query.ToArray);
        }

        public static T ToCachedFirstOrDefault<T>(this IQueryable<T> query, string cacheKey = null)
        {
            return CacheManager.Get(cacheKey ?? query.ToString(), query.FirstOrDefault);
        }

        public static IPagedList<T> ToCachedPagedList<T>(this IQueryable<T> query, string cacheKey, int pageIndex, int pageSize)
        {
            return CacheManager.Get(cacheKey ?? query.ToString(), () => new PagedList<T>(query, pageIndex, pageSize));
        }
    }
}

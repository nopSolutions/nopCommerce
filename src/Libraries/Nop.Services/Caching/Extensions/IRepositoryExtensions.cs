using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Data;

namespace Nop.Services.Caching.Extensions
{
    public static class IRepositoryExtensions
    {
        /// <summary>
        /// Gets a cached element by Id
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <param name="repository">Entity repository</param>
        /// <param name="id">Entity identifier</param>
        /// <param name="cacheTime">Cache time in minutes; set null to use default time</param>
        /// <returns>Cached entity</returns>
        public static TEntity ToCachedGetById<TEntity>(this IRepository<TEntity> repository, object id, int? cacheTime=null) where TEntity : BaseEntity
        {
            var staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();

            var cacheKey = new CacheKey(BaseEntity.GetEntityCacheKey(typeof(TEntity), id));

            if (cacheTime.HasValue) 
                cacheKey.CacheTime = cacheTime.Value;
            else
            {
                var cacheKeyService = EngineContext.Current.Resolve<ICacheKeyService>();
                cacheKey = cacheKeyService.PrepareKeyForDefaultCache(cacheKey);
            }

            return staticCacheManager.Get(cacheKey, () => repository.GetById(id));
        }
    }
}

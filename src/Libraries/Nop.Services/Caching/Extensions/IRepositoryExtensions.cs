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
        /// <returns>Cached entity</returns>
        public static TEntity ToCachedGetById<TEntity>(this IRepository<TEntity> repository, object id) where TEntity : BaseEntity
        {
            var cacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();

            return cacheManager.Get(new CacheKey(BaseEntity.GetEntityCacheKey(typeof(TEntity), id)), () => repository.GetById(id));
        }
    }
}

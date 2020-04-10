using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Data;

namespace Nop.Services.Caching.Extensions
{
    public static class IRepositoryExtensions
    {
        public static TEntity ToCachedGetById<TEntity>(this IRepository<TEntity> repository, object id) where TEntity : BaseEntity
        {
            var cacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();

            return cacheManager.Get(new CacheKey(BaseEntity.GetEntityCacheKey(typeof(TEntity), id)), () => repository.GetById(id));
        }
    }
}

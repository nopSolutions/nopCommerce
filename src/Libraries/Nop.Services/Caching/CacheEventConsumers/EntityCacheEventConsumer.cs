using Nop.Core;
using Nop.Core.Caching;

namespace Nop.Services.Caching.CacheEventConsumers
{
    public abstract class EntityCacheEventConsumer<TEntity> : CacheEventConsumer<TEntity> where TEntity : BaseEntity
    {
        public override void ClearCashe(TEntity entity)
        {
            _cacheManager.Remove(string.Format(NopCachingDefaults.NopEntityCacheKey, typeof(TEntity).Name, entity.Id));
        }
    }
}
using Nop.Core;
using Nop.Core.Caching;

namespace Nop.Services.Caching.CacheEventConsumers
{
    public abstract class EntityCacheEventConsumer<T> : CacheEventConsumer<T> where T : BaseEntity
    {
        public override void ClearCashe(T entity)
        {
            _cacheManager.RemoveByPrefix(string.Format(NopCachingDefaults.NopEntityPrefixCacheKey, typeof(T).Name));
        }
    }
}
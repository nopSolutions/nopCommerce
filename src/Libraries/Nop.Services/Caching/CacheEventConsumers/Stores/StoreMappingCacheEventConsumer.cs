using Nop.Core.Domain.Stores;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Stores
{
    /// <summary>
    /// Represents a store mapping cache event consumer
    /// </summary>
    public partial class StoreMappingCacheEventConsumer : CacheEventConsumer<StoreMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(StoreMapping entity)
        {
            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var key = NopStoreCachingDefaults.StoreMappingsByEntityIdNameCacheKey.ToCacheKey(entityId, entityName);

            Remove(key);

            key = NopStoreCachingDefaults.StoreMappingIdsByEntityIdNameCacheKey.ToCacheKey(entityId, entityName);
            
            Remove(key);
        }
    }
}

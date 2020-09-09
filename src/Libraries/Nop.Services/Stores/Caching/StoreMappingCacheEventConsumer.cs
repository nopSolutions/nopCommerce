using Nop.Core.Domain.Stores;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Stores.Caching
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
        protected override async Task ClearCache(StoreMapping entity)
        {
            var entityId = entity.EntityId;
            var entityName = entity.EntityName;

            var key = _cacheKeyService.PrepareKey(NopStoreDefaults.StoreMappingsByEntityIdNameCacheKey, entityId, entityName);

            await Remove(key);

            key = _cacheKeyService.PrepareKey(NopStoreDefaults.StoreMappingIdsByEntityIdNameCacheKey, entityId, entityName);
            
            await Remove(key);
        }
    }
}

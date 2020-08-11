using Nop.Core.Domain.Stores;
using Nop.Services.Caching;

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
        protected override void ClearCache(StoreMapping entity)
        {
            Remove(NopStoreDefaults.StoreMappingsCacheKey, entity.EntityId, entity.EntityName);
            Remove(NopStoreDefaults.StoreMappingIdsCacheKey, entity.EntityId, entity.EntityName);
        }
    }
}

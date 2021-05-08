using System.Threading.Tasks;
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
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(StoreMapping entity)
        {
            await RemoveAsync(NopStoreDefaults.StoreMappingsCacheKey, entity.EntityId, entity.EntityName);
            await RemoveAsync(NopStoreDefaults.StoreMappingIdsCacheKey, entity.EntityId, entity.EntityName);
            await RemoveAsync(NopStoreDefaults.StoreMappingExistsCacheKey, entity.EntityName);
        }
    }
}

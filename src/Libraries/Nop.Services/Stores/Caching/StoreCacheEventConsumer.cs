using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Services.Caching;
using Nop.Services.Localization;

namespace Nop.Services.Stores.Caching
{
    /// <summary>
    /// Represents a store cache event consumer
    /// </summary>
    public partial class StoreCacheEventConsumer : CacheEventConsumer<Store>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Store entity)
        {
            RemoveByPrefix(NopEntityCacheDefaults<ShoppingCartItem>.AllPrefix);
            RemoveByPrefix(NopLocalizationDefaults.LanguagesByStorePrefix, entity);
        }
    }
}

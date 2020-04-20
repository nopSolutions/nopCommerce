using Nop.Core.Domain.Stores;
using Nop.Services.Caching;
using Nop.Services.Localization;
using Nop.Services.Orders;

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
            Remove(NopStoreDefaults.StoresAllCacheKey);
            RemoveByPrefix(NopOrderDefaults.ShoppingCartPrefixCacheKey);

            var prefix = _cacheKeyService.PrepareKeyPrefix(NopLocalizationDefaults.LanguagesByStoreIdPrefixCacheKey, entity);

            RemoveByPrefix(prefix);
        }
    }
}

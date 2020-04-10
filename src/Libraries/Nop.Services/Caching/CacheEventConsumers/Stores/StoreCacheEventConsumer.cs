using Nop.Core.Domain.Stores;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Stores
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
            Remove(NopStoreCachingDefaults.StoresAllCacheKey);
            RemoveByPrefix(NopOrderCachingDefaults.ShoppingCartPrefixCacheKey, false);

            var prefix = NopLocalizationCachingDefaults.LanguagesByStoreIdPrefixCacheKey.ToCacheKey(entity);

            RemoveByPrefix(prefix);
        }
    }
}

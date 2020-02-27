using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a tier price cache event consumer
    /// </summary>
    public partial class TierPriceCacheEventConsumer : CacheEventConsumer<TierPrice>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(TierPrice entity)
        {
            var cacheKey = NopCatalogCachingDefaults.ProductTierPrices.ToCacheKey(entity.ProductId);
            Remove(cacheKey);

            var prefix = NopCatalogCachingDefaults.ProductPricePrefixCacheKey.ToCacheKey(entity.ProductId);
            RemoveByPrefix(prefix);
        }
    }
}

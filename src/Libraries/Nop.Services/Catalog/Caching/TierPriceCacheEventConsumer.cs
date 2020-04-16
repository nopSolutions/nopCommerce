using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
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
            var cacheKey = NopCatalogDefaults.ProductTierPricesCacheKey.FillCacheKey(entity.ProductId);
            Remove(cacheKey);

            var prefix = NopCatalogDefaults.ProductPricePrefixCacheKey.ToCacheKey(entity.ProductId);
            RemoveByPrefix(prefix);
        }
    }
}

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
            Remove(NopCatalogDefaults.TierPricesByProductCacheKey, entity.ProductId);
            RemoveByPrefix(NopCatalogDefaults.ProductPricePrefix, entity.ProductId);
        }
    }
}

using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a manufacturer cache event consumer
    /// </summary>
    public partial class ManufacturerCacheEventConsumer : CacheEventConsumer<Manufacturer>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Manufacturer entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ManufacturersPrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductManufacturersPrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductManufacturerIdsPrefixCacheKey);
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }
    }
}

using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a product warehouse inventory cache event consumer
    /// </summary>
    public partial class ProductWarehouseInventoryCacheEventConsumer : CacheEventConsumer<ProductWarehouseInventory>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ProductWarehouseInventory entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ProductsPrefixCacheKey);
        }
    }
}

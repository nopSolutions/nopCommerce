using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ProductWarehouseInventoryCacheEventConsumer : CacheEventConsumer<ProductWarehouseInventory>
    {
        protected override void ClearCache(ProductWarehouseInventory entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ProductsPrefixCacheKey);
        }
    }
}

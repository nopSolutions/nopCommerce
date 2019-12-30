using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ProductManufacturerCacheEventConsumer : CacheEventConsumer<ProductManufacturer>
    {
        public override void ClearCashe(ProductManufacturer entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ManufacturersPrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductManufacturersPrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductPricePrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductManufacturerIdsPrefixCacheKey);
        }
    }
}

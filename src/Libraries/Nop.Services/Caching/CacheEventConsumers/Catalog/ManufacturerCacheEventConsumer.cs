using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ManufacturerCacheEventConsumer : CacheEventConsumer<Manufacturer>
    {
        public override void ClearCashe(Manufacturer entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ManufacturersPrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductManufacturersPrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductManufacturerIdsPrefixCacheKey);
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }
    }
}

using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ManufacturerCacheEventConsumer : CacheEventConsumer<Manufacturer>
    {
        public override void ClearCashe(Manufacturer entity)
        {
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ManufacturersPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductManufacturersPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductManufacturerIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountCachingDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }
    }
}

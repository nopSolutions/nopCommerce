using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ProductCacheEventConsumer : CacheEventConsumer<Product>
    {
        public override void ClearCashe(Product entity)
        {
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductPricePrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopOrderCachingDefaults.ShoppingCartPrefixCacheKey);
        }
    }
}

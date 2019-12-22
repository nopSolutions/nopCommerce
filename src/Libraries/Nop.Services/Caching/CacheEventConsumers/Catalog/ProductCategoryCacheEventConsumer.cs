using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ProductCategoryCacheEventConsumer : CacheEventConsumer<ProductCategory>
    {
        public override void ClearCashe(ProductCategory entity)
        {
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductCategoriesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductPricePrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductCategoryIdsPrefixCacheKey);
        }
    }
}

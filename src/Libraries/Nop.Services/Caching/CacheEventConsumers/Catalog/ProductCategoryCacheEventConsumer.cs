using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ProductCategoryCacheEventConsumer : CacheEventConsumer<ProductCategory>
    {
        protected override void ClearCache(ProductCategory entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ProductCategoriesPrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductPricePrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductCategoryIdsPrefixCacheKey);
        }
    }
}

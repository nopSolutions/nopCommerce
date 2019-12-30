using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class CategoryCacheEventConsumer : CacheEventConsumer<Category>
    {
        public override void ClearCache(Category entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.CategoriesPrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductCategoriesPrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductCategoryIdsPrefixCacheKey);
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountCategoryIdsPrefixCacheKey);
        }
    }
}

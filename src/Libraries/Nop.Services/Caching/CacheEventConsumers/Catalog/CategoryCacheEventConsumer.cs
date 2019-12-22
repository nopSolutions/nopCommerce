using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class CategoryCacheEventConsumer : CacheEventConsumer<Category>
    {
        public override void ClearCashe(Category entity)
        {
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.CategoriesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductCategoriesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductCategoryIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountCachingDefaults.DiscountCategoryIdsPrefixCacheKey);
        }
    }
}

using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a category cache event consumer
    /// </summary>
    public partial class CategoryCacheEventConsumer : CacheEventConsumer<Category>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Category entity)
        {
            var prefix = NopCatalogCachingDefaults.CategoriesByParentCategoryPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);
            prefix = NopCatalogCachingDefaults.CategoriesByParentCategoryPrefixCacheKey.ToCacheKey(entity.ParentCategoryId);
            RemoveByPrefix(prefix);

            prefix = NopCatalogCachingDefaults.CategoriesChildIdentifiersPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);
            prefix = NopCatalogCachingDefaults.CategoriesChildIdentifiersPrefixCacheKey.ToCacheKey(entity.ParentCategoryId);
            RemoveByPrefix(prefix);

            prefix = NopCatalogCachingDefaults.ProductCategoriesByCategoryPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);

            RemoveByPrefix(NopCatalogCachingDefaults.CategoriesDisplayedOnHomepagePrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.CategoriesAllPrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.CategoryBreadcrumbPrefixCacheKey);
            
            RemoveByPrefix(NopCatalogCachingDefaults.CategoryNumberOfProductsPrefixCacheKey);

            RemoveByPrefix(NopDiscountCachingDefaults.DiscountCategoryIdsPrefixCacheKey);
        }
    }
}

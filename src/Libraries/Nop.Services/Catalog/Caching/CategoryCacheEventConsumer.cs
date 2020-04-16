using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;
using Nop.Services.Discounts;

namespace Nop.Services.Catalog.Caching
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
            var prefix = NopCatalogDefaults.CategoriesByParentCategoryPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);
            prefix = NopCatalogDefaults.CategoriesByParentCategoryPrefixCacheKey.ToCacheKey(entity.ParentCategoryId);
            RemoveByPrefix(prefix);

            prefix = NopCatalogDefaults.CategoriesChildIdentifiersPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);
            prefix = NopCatalogDefaults.CategoriesChildIdentifiersPrefixCacheKey.ToCacheKey(entity.ParentCategoryId);
            RemoveByPrefix(prefix);
            
            RemoveByPrefix(NopCatalogDefaults.CategoriesDisplayedOnHomepagePrefixCacheKey);
            RemoveByPrefix(NopCatalogDefaults.CategoriesAllPrefixCacheKey);
            RemoveByPrefix(NopCatalogDefaults.CategoryBreadcrumbPrefixCacheKey);
            
            RemoveByPrefix(NopCatalogDefaults.CategoryNumberOfProductsPrefixCacheKey);

            RemoveByPrefix(NopDiscountDefaults.DiscountCategoryIdsPrefixCacheKey);
        }
    }
}

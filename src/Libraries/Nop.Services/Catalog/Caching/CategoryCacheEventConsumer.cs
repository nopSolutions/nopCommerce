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
            var prefix = _cacheKeyService.PrepareKeyPrefix(NopCatalogDefaults.CategoriesByParentCategoryPrefixCacheKey, entity);
            RemoveByPrefix(prefix);
            prefix = _cacheKeyService.PrepareKeyPrefix(NopCatalogDefaults.CategoriesByParentCategoryPrefixCacheKey, entity.ParentCategoryId);
            RemoveByPrefix(prefix);

            prefix = _cacheKeyService.PrepareKeyPrefix(NopCatalogDefaults.CategoriesChildIdentifiersPrefixCacheKey, entity);
            RemoveByPrefix(prefix);
            prefix = _cacheKeyService.PrepareKeyPrefix(NopCatalogDefaults.CategoriesChildIdentifiersPrefixCacheKey, entity.ParentCategoryId);
            RemoveByPrefix(prefix);
            
            RemoveByPrefix(NopCatalogDefaults.CategoriesDisplayedOnHomepagePrefixCacheKey);
            RemoveByPrefix(NopCatalogDefaults.CategoriesAllPrefixCacheKey);
            RemoveByPrefix(NopCatalogDefaults.CategoryBreadcrumbPrefixCacheKey);
            
            RemoveByPrefix(NopCatalogDefaults.CategoryNumberOfProductsPrefixCacheKey);

            RemoveByPrefix(NopDiscountDefaults.DiscountCategoryIdsPrefixCacheKey);
        }
    }
}

using System.Threading.Tasks;
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
        protected override async Task ClearCache(Category entity)
        {
            var prefix = _cacheKeyService.PrepareKeyPrefix(NopCatalogDefaults.CategoriesByParentCategoryPrefixCacheKey, entity);
              await RemoveByPrefix(prefix);
            prefix = _cacheKeyService.PrepareKeyPrefix(NopCatalogDefaults.CategoriesByParentCategoryPrefixCacheKey, entity.ParentCategoryId);
              await RemoveByPrefix(prefix);

            prefix = _cacheKeyService.PrepareKeyPrefix(NopCatalogDefaults.CategoriesChildIdentifiersPrefixCacheKey, entity);
              await RemoveByPrefix(prefix);
            prefix = _cacheKeyService.PrepareKeyPrefix(NopCatalogDefaults.CategoriesChildIdentifiersPrefixCacheKey, entity.ParentCategoryId);
              await RemoveByPrefix(prefix);
            
            await RemoveByPrefix(NopCatalogDefaults.CategoriesDisplayedOnHomepagePrefixCacheKey);
            await RemoveByPrefix(NopCatalogDefaults.CategoriesAllPrefixCacheKey);
            await RemoveByPrefix(NopCatalogDefaults.CategoryBreadcrumbPrefixCacheKey);
            
            await RemoveByPrefix(NopCatalogDefaults.CategoryNumberOfProductsPrefixCacheKey);

            await RemoveByPrefix(NopDiscountDefaults.DiscountCategoryIdsPrefixCacheKey);
        }
    }
}

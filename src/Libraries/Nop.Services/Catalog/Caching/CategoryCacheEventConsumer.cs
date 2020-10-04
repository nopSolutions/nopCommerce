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
            RemoveByPrefix(NopCatalogDefaults.CategoriesByParentCategoryPrefix, entity);
            RemoveByPrefix(NopCatalogDefaults.CategoriesByParentCategoryPrefix, entity.ParentCategoryId);
            RemoveByPrefix(NopCatalogDefaults.CategoriesChildIdsPrefix, entity);
            RemoveByPrefix(NopCatalogDefaults.CategoriesChildIdsPrefix, entity.ParentCategoryId);
            RemoveByPrefix(NopCatalogDefaults.CategoriesHomepagePrefix);
            RemoveByPrefix(NopCatalogDefaults.CategoryBreadcrumbPrefix);
            RemoveByPrefix(NopCatalogDefaults.CategoryProductsNumberPrefix);
            RemoveByPrefix(NopDiscountDefaults.CategoryIdsPrefix);
        }
    }
}

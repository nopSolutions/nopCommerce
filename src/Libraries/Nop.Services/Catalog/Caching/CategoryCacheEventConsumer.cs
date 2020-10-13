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
            await RemoveByPrefix(NopCatalogDefaults.CategoriesByParentCategoryPrefix, entity);
            await RemoveByPrefix(NopCatalogDefaults.CategoriesByParentCategoryPrefix, entity.ParentCategoryId);
            await RemoveByPrefix(NopCatalogDefaults.CategoriesChildIdsPrefix, entity);
            await RemoveByPrefix(NopCatalogDefaults.CategoriesChildIdsPrefix, entity.ParentCategoryId);
            await RemoveByPrefix(NopCatalogDefaults.CategoriesHomepagePrefix);
            await RemoveByPrefix(NopCatalogDefaults.CategoryBreadcrumbPrefix);
            await RemoveByPrefix(NopCatalogDefaults.CategoryProductsNumberPrefix);
            await RemoveByPrefix(NopDiscountDefaults.CategoryIdsPrefix);
        }
    }
}

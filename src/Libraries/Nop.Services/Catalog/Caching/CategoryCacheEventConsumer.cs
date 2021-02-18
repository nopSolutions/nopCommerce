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
        protected override async Task ClearCacheAsync(Category entity)
        {
            await RemoveByPrefixAsync(NopCatalogDefaults.CategoriesByParentCategoryPrefix, entity);
            await RemoveByPrefixAsync(NopCatalogDefaults.CategoriesByParentCategoryPrefix, entity.ParentCategoryId);
            await RemoveByPrefixAsync(NopCatalogDefaults.CategoriesChildIdsPrefix, entity);
            await RemoveByPrefixAsync(NopCatalogDefaults.CategoriesChildIdsPrefix, entity.ParentCategoryId);
            await RemoveByPrefixAsync(NopCatalogDefaults.CategoriesHomepagePrefix);
            await RemoveByPrefixAsync(NopCatalogDefaults.CategoryBreadcrumbPrefix);
            await RemoveByPrefixAsync(NopCatalogDefaults.CategoryProductsNumberPrefix);
            await RemoveByPrefixAsync(NopDiscountDefaults.CategoryIdsPrefix);
        }
    }
}

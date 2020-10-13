using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product category cache event consumer
    /// </summary>
    public partial class ProductCategoryCacheEventConsumer : CacheEventConsumer<ProductCategory>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCache(ProductCategory entity)
        {
            await RemoveByPrefix(NopCatalogDefaults.ProductCategoriesByProductPrefix, entity.ProductId);
            await RemoveByPrefix(NopCatalogDefaults.CategoryProductsNumberPrefix);
            await RemoveByPrefix(NopCatalogDefaults.ProductPricePrefix, entity.ProductId);
        }
    }
}

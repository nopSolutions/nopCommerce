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
        protected override void ClearCache(ProductCategory entity)
        {
            RemoveByPrefix(_staticCacheManager.PrepareKeyPrefix(NopCatalogDefaults.ProductCategoriesByProductPrefix, entity.ProductId));
            RemoveByPrefix(NopCatalogDefaults.CategoryProductsNumberPrefix);
            RemoveByPrefix(_staticCacheManager.PrepareKeyPrefix(NopCatalogDefaults.ProductPricePrefix, entity.ProductId));
        }
    }
}

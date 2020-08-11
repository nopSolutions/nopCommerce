using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product attribute combination cache event consumer
    /// </summary>
    public partial class ProductAttributeCombinationCacheEventConsumer : CacheEventConsumer<ProductAttributeCombination>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ProductAttributeCombination entity)
        {
            Remove(_staticCacheManager.PrepareKey(NopCatalogDefaults.ProductAttributeMappingsByProductCacheKey, entity.ProductId));
            Remove(_staticCacheManager.PrepareKey(NopCatalogDefaults.ProductAttributeCombinationsByProductCacheKey, entity.ProductId));
        }
    }
}

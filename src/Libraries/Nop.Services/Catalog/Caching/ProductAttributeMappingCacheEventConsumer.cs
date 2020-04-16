using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product attribute mapping cache event consumer
    /// </summary>
    public partial class ProductAttributeMappingCacheEventConsumer : CacheEventConsumer<ProductAttributeMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ProductAttributeMapping entity)
        {
            var cacheKey = NopCatalogDefaults.ProductAttributeMappingsAllCacheKey.FillCacheKey(entity.ProductId);
            Remove(cacheKey);

            cacheKey = NopCatalogDefaults.ProductAttributeValuesAllCacheKey.FillCacheKey(entity);
            Remove(cacheKey);

            cacheKey = NopCatalogDefaults.ProductAttributeCombinationsAllCacheKey.FillCacheKey(entity.ProductId);
            Remove(cacheKey);
        }
    }
}

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
            var cacheKey = _cacheKeyService.PrepareKey(NopCatalogDefaults.ProductAttributeMappingsAllCacheKey, entity.ProductId);
            Remove(cacheKey);

            cacheKey = _cacheKeyService.PrepareKey(NopCatalogDefaults.ProductAttributeValuesAllCacheKey, entity);
            Remove(cacheKey);

            cacheKey = _cacheKeyService.PrepareKey(NopCatalogDefaults.ProductAttributeCombinationsAllCacheKey, entity.ProductId);
            Remove(cacheKey);
        }
    }
}

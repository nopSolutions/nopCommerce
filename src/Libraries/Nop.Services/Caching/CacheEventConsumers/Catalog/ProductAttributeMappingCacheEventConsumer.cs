using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
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
            var cacheKey = NopCatalogCachingDefaults.ProductsByProductAtributeCacheKey.FillCacheKey(entity.ProductAttributeId);
            Remove(cacheKey);

            cacheKey = NopCatalogCachingDefaults.ProductAttributeMappingsAllCacheKey.FillCacheKey(entity.ProductId);
            Remove(cacheKey);

            cacheKey = NopCatalogCachingDefaults.ProductAttributeValuesAllCacheKey.FillCacheKey(entity);
            Remove(cacheKey);

            RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributesAllPrefixCacheKey);

            cacheKey = NopCatalogCachingDefaults.ProductAttributeCombinationsAllCacheKey.FillCacheKey(entity.ProductId);
            Remove(cacheKey);
        }
    }
}

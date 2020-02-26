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
            var cacheKey = NopCatalogCachingDefaults.ProductsByProductAtributeCacheKey.ToCacheKey(entity.ProductAttributeId);
            Remove(cacheKey);

            cacheKey = NopCatalogCachingDefaults.ProductAttributeMappingsAllCacheKey.ToCacheKey(entity.ProductId);
            Remove(cacheKey);

            cacheKey = NopCatalogCachingDefaults.ProductAttributeValuesAllCacheKey.ToCacheKey(entity);
            Remove(cacheKey);

            RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributesAllPrefixCacheKey);

            cacheKey = NopCatalogCachingDefaults.ProductAttributeCombinationsAllCacheKey.ToCacheKey(entity.ProductId);
            Remove(cacheKey);
        }
    }
}

using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a product attribute cache event consumer
    /// </summary>
    public partial class ProductAttributeCacheEventConsumer : CacheEventConsumer<ProductAttribute>
    {
        /// <summary>
        /// entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override void ClearCache(ProductAttribute entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Delete)
            {
                RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributeMappingsPrefixCacheKey);
                RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributeValuesAllPrefixCacheKey);
                RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributeCombinationsAllPrefixCacheKey);
            }

            RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributesAllPrefixCacheKey);

            var cacheKey = NopCatalogCachingDefaults.ProductsByProductAtributeCacheKey.FillCacheKey(entity);
            Remove(cacheKey);
        }
    }
}

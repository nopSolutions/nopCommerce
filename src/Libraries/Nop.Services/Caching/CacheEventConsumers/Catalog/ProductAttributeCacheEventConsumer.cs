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
                RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributeValuesPrefixCacheKey);
                RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributeCombinationsPrefixCacheKey);
            }

            RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributesAllPrefixCacheKey);

            var cacheKey = NopCatalogCachingDefaults.ProductsByProductAtributeCacheKey.ToCacheKey(entity);
            Remove(cacheKey);
        }
    }
}

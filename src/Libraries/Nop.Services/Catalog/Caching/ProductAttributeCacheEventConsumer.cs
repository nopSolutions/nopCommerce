using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
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
            if (entityEventType != EntityEventType.Delete) 
                return;

            RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesAllPrefixCacheKey);
            RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsAllPrefixCacheKey);
        }
    }
}

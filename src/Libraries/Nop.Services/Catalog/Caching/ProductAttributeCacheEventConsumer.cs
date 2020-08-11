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
            if (entityEventType != EntityEventType.Insert)
                return;

            Remove(_staticCacheManager.PrepareKey(NopCatalogDefaults.ProductAttributeValuesByAttributeCacheKey, entity));
        }
    }
}

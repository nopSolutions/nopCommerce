using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a product template cache event consumer
    /// </summary>
    public partial class ProductTemplateCacheEventConsumer : CacheEventConsumer<ProductTemplate>
    {
        /// <summary>
        /// entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override void ClearCache(ProductTemplate entity, EntityEventType entityEventType)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ProductTemplatesPrefixCacheKey);
        }
    }
}

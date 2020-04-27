using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
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
            Remove(NopCatalogDefaults.ProductTemplatesAllCacheKey);
        }
    }
}

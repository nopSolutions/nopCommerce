using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a specification attribute cache event consumer
    /// </summary>
    public partial class SpecificationAttributeCacheEventConsumer : CacheEventConsumer<SpecificationAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override void ClearCache(SpecificationAttribute entity, EntityEventType entityEventType)
        {
            Remove(NopCatalogDefaults.SpecificationAttributesWithOptionsCacheKey);

            if (entityEventType != EntityEventType.Insert)
            {
                RemoveByPrefix(NopCatalogDefaults.ProductSpecificationAttributeAllByProductPrefix);
                RemoveByPrefix(NopCatalogDefaults.SpecificationAttributeGroupByProductPrefix);
            }
        }
    }
}

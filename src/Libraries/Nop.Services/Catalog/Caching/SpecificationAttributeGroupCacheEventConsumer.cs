using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a specification attribute group cache event consumer
    /// </summary>
    public partial class SpecificationAttributeGroupCacheEventConsumer : CacheEventConsumer<SpecificationAttributeGroup>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override void ClearCache(SpecificationAttributeGroup entity, EntityEventType entityEventType)
        {
            if (entityEventType != EntityEventType.Insert)
                RemoveByPrefix(NopCatalogDefaults.SpecificationAttributeGroupByProductPrefix);
        }
    }
}

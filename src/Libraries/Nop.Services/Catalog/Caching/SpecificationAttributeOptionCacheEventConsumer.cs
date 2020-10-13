using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a specification attribute option cache event consumer
    /// </summary>
    public partial class SpecificationAttributeOptionCacheEventConsumer : CacheEventConsumer<SpecificationAttributeOption>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override async Task ClearCache(SpecificationAttributeOption entity, EntityEventType entityEventType)
        {
            await Remove(NopCatalogDefaults.SpecificationAttributesWithOptionsCacheKey);
            await Remove(NopCatalogDefaults.SpecificationAttributeOptionsCacheKey, entity.SpecificationAttributeId);
            await RemoveByPrefix(NopCatalogDefaults.ProductSpecificationAttributeAllByProductPrefix);

            if (entityEventType == EntityEventType.Delete)
                await RemoveByPrefix(NopCatalogDefaults.SpecificationAttributeGroupByProductPrefix);
        }
    }
}

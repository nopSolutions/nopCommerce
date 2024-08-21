using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching;

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
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(SpecificationAttributeOption entity, EntityEventType entityEventType)
    {
        await RemoveAsync(NopCatalogDefaults.SpecificationAttributesWithOptionsCacheKey);
        await RemoveAsync(NopCatalogDefaults.SpecificationAttributeOptionsCacheKey, entity.SpecificationAttributeId);
        await RemoveByPrefixAsync(NopCatalogDefaults.ProductSpecificationAttributeAllByProductPrefix);
        await RemoveByPrefixAsync(NopCatalogDefaults.FilterableSpecificationAttributeOptionsPrefix);

        if (entityEventType == EntityEventType.Delete)
            await RemoveByPrefixAsync(NopCatalogDefaults.SpecificationAttributeGroupByProductPrefix);

        await base.ClearCacheAsync(entity, entityEventType);
    }
}
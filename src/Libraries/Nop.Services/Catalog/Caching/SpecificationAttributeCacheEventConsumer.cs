using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching;

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
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(SpecificationAttribute entity, EntityEventType entityEventType)
    {
        await RemoveAsync(NopCatalogDefaults.SpecificationAttributesWithOptionsCacheKey);

        if (entityEventType != EntityEventType.Insert)
        {
            await RemoveByPrefixAsync(NopCatalogDefaults.ProductSpecificationAttributeAllByProductPrefix);
            await RemoveByPrefixAsync(NopCatalogDefaults.SpecificationAttributeGroupByProductPrefix);
            await RemoveByPrefixAsync(NopCatalogDefaults.FilterableSpecificationAttributeOptionsPrefix);
        }

        await base.ClearCacheAsync(entity, entityEventType);
    }
}
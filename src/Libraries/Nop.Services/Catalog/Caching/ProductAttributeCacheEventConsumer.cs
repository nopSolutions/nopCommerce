using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a product attribute cache event consumer
/// </summary>
public partial class ProductAttributeCacheEventConsumer : CacheEventConsumer<ProductAttribute>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(ProductAttribute entity, EntityEventType entityEventType)
    {
        if (entityEventType == EntityEventType.Insert)
            await RemoveAsync(NopCatalogDefaults.ProductAttributeValuesByAttributeCacheKey, entity);

        if (entityEventType == EntityEventType.Delete)
        {
            await RemoveByPrefixAsync(NopCatalogDefaults.ProductAttributeMappingsByProductPrefix);
            await RemoveByPrefixAsync(NopEntityCacheDefaults<ProductAttributeMapping>.ByIdPrefix);
        }
        await base.ClearCacheAsync(entity, entityEventType);
    }
}
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a product attribute value cache event consumer
/// </summary>
public partial class ProductAttributeValueCacheEventConsumer : CacheEventConsumer<ProductAttributeValue>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(ProductAttributeValue entity)
    {
        await RemoveAsync(NopCatalogDefaults.ProductAttributeValuesByAttributeCacheKey, entity.ProductAttributeMappingId);
    }
}
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a product review review type cache event consumer
/// </summary>
public partial class ProductReviewReviewTypeMappingCacheEventConsumer : CacheEventConsumer<ProductReviewReviewTypeMapping>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(ProductReviewReviewTypeMapping entity)
    {
        await RemoveAsync(NopCatalogDefaults.ProductReviewTypeMappingByReviewTypeCacheKey, entity.ProductReviewId);
    }
}
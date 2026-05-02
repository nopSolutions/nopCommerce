using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a product review cache event consumer
/// </summary>
public partial class ProductReviewCacheEventConsumer : CacheEventConsumer<ProductReview>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(ProductReview entity)
    {
        await RemoveAsync(NopCatalogDefaults.ProductReviewTypeMappingByReviewIdCacheKey, entity.Id);
        await base.ClearCacheAsync(entity);
    }
}
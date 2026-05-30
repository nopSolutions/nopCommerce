using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a review type cache event consumer
/// </summary>
public partial class ReviewTypeCacheEventConsumer : CacheEventConsumer<ReviewType>
{
    public override async Task HandleEventAsync(EntityDeletedEvent<ReviewType> eventMessage)
    {
        await RemoveByPrefixAsync(NopCatalogDefaults.ProductReviewTypeMappingByReviewIdPrefix);
        await base.HandleEventAsync(eventMessage);
    }
}
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a review type cache event consumer
    /// </summary>
    public partial class ReviewTypeCacheEventConsumer : CacheEventConsumer<ReviewType>
    {
        /// <summary>
        /// entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override void ClearCache(ReviewType entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Delete)
                RemoveByPrefix(NopCatalogCachingDefaults.ProductReviewReviewTypeMappingPrefixCacheKey);

            Remove(NopCatalogCachingDefaults.ReviewTypeAllKey);
        }
    }
}

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
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ReviewType entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ReviewTypeByPrefixCacheKey);
        }
    }
}

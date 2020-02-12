using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a product review cache event consumer
    /// </summary>
    public partial class ProductReviewCacheEventConsumer : CacheEventConsumer<ProductReview>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ProductReview entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ProductsPrefixCacheKey);
        }
    }
}

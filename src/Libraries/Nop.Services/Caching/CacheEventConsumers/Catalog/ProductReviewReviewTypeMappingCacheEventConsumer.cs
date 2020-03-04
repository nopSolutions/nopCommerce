using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a product review review type cache event consumer
    /// </summary>
    public partial class ProductReviewReviewTypeMappingCacheEventConsumer : CacheEventConsumer<ProductReviewReviewTypeMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ProductReviewReviewTypeMapping entity)
        {
            Remove(NopCatalogCachingDefaults.ReviewTypeAllCacheKey);

            var cacheKey = NopCatalogCachingDefaults.ProductReviewReviewTypeMappingAllCacheKey.FillCacheKey(entity.ProductReviewId);
            Remove(cacheKey);
        }
    }
}

using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ProductReviewReviewTypeMappingCacheEventConsumer : CacheEventConsumer<ProductReviewReviewTypeMapping>
    {
        protected override void ClearCache(ProductReviewReviewTypeMapping entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ReviewTypeByPrefixCacheKey);
        }
    }
}

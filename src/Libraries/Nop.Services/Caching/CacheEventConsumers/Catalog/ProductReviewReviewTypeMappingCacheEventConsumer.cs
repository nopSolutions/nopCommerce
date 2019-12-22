using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ProductReviewReviewTypeMappingCacheEventConsumer : CacheEventConsumer<ProductReviewReviewTypeMapping>
    {
        public override void ClearCashe(ProductReviewReviewTypeMapping entity)
        {
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ReviewTypeByPrefixCacheKey);
        }
    }
}

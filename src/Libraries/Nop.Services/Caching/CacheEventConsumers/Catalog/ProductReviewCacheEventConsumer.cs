using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ProductReviewCacheEventConsumer : CacheEventConsumer<ProductReview>
    {
        public override void ClearCashe(ProductReview entity)
        {
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductsPrefixCacheKey);
        }
    }
}

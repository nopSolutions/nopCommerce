using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ReviewTypeCacheEventConsumer : CacheEventConsumer<ReviewType>
    {
        public override void ClearCache(ReviewType entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ReviewTypeByPrefixCacheKey);
        }
    }
}

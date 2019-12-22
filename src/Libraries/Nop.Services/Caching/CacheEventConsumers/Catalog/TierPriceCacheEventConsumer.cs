using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class TierPriceCacheEventConsumer : CacheEventConsumer<TierPrice>
    {
        public override void ClearCashe(TierPrice entity)
        {
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductPricePrefixCacheKey);
        }
    }
}

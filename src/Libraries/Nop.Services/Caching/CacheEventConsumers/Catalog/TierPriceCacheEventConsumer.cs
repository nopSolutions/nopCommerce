using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class TierPriceCacheEventConsumer : CacheEventConsumer<TierPrice>
    {
        public override void ClearCashe(TierPrice entity)
        {
           RemoveByPrefix(NopCatalogCachingDefaults.ProductsPrefixCacheKey);
           RemoveByPrefix(NopCatalogCachingDefaults.ProductPricePrefixCacheKey);
        }
    }
}

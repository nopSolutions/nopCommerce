using Nop.Core.Domain.Stores;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Stores
{
    public partial class StoreCacheEventConsumer : CacheEventConsumer<Store>
    {
        public override void ClearCashe(Store entity)
        {
            _cacheManager.RemoveByPrefix(NopStoreCachingDefaults.StoresPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopOrderCachingDefaults.ShoppingCartPrefixCacheKey);
        }
    }
}

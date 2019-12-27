using Nop.Core.Domain.Stores;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Stores
{
    public partial class StoreCacheEventConsumer : CacheEventConsumer<Store>
    {
        public override void ClearCashe(Store entity)
        {
            RemoveByPrefix(NopStoreCachingDefaults.StoresPrefixCacheKey);
            RemoveByPrefix(NopOrderCachingDefaults.ShoppingCartPrefixCacheKey);
        }
    }
}

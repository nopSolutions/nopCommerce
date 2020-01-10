using Nop.Core.Domain.Stores;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Stores
{
    public partial class StoreCacheEventConsumer : CacheEventConsumer<Store>
    {
        protected override void ClearCache(Store entity)
        {
            RemoveByPrefix(NopStoreCachingDefaults.StoresPrefixCacheKey);
            RemoveByPrefix(NopNewsCachingDefaults.ShoppingCartPrefixCacheKey, false);
            RemoveByPrefix(NopLocalizationCachingDefaults.LanguagesPrefixCacheKey);
        }
    }
}

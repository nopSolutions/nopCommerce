using Nop.Core.Domain.Stores;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Stores
{
    public partial class StoreMappingCacheEventConsumer : CacheEventConsumer<StoreMapping>
    {
        protected override void ClearCache(StoreMapping entity)
        {
            RemoveByPrefix(NopStoreCachingDefaults.StoreMappingPrefixCacheKey);
        }
    }
}

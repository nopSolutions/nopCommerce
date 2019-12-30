using Nop.Core.Domain.Stores;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Stores
{
    public partial class StoreMappingCacheEventConsumer : CacheEventConsumer<StoreMapping>
    {
        public override void ClearCashe(StoreMapping entity)
        {
            RemoveByPrefix(NopStoreCachingDefaults.StoreMappingPrefixCacheKey);
        }
    }
}

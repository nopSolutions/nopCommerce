using Nop.Core.Domain.Directory;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Directory
{
    public partial class StateProvinceCacheEventConsumer : CacheEventConsumer<StateProvince>
    {
        public override void ClearCache(StateProvince entity)
        {
            RemoveByPrefix(NopDirectoryCachingDefaults.StateProvincesPrefixCacheKey);
        }
    }
}

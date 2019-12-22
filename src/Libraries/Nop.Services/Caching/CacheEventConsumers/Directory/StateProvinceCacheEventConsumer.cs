using Nop.Core.Domain.Directory;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Directory
{
    public partial class StateProvinceCacheEventConsumer : CacheEventConsumer<StateProvince>
    {
        public override void ClearCashe(StateProvince entity)
        {
            _cacheManager.RemoveByPrefix(NopDirectoryCachingDefaults.StateProvincesPrefixCacheKey);
        }
    }
}

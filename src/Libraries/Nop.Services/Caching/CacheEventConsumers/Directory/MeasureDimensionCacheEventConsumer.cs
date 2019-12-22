using Nop.Core.Domain.Directory;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Directory
{
    public partial class MeasureDimensionCacheEventConsumer : CacheEventConsumer<MeasureDimension>
    {
        public override void ClearCashe(MeasureDimension entity)
        {
            _cacheManager.RemoveByPrefix(NopDirectoryCachingDefaults.MeasureDimensionsPrefixCacheKey);
        }
    }
}

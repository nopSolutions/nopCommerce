using Nop.Core.Domain.Directory;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Directory
{
    public partial class MeasureWeightCacheEventConsumer : CacheEventConsumer<MeasureWeight>
    {
        public override void ClearCashe(MeasureWeight entity)
        {
            _cacheManager.RemoveByPrefix(NopDirectoryCachingDefaults.MeasureDimensionsPrefixCacheKey);
        }
    }
}

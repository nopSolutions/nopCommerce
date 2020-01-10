using Nop.Core.Domain.Directory;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Directory
{
    public partial class MeasureDimensionCacheEventConsumer : CacheEventConsumer<MeasureDimension>
    {
        protected override void ClearCache(MeasureDimension entity)
        {
            RemoveByPrefix(NopDirectoryCachingDefaults.MeasureDimensionsPrefixCacheKey);
        }
    }
}

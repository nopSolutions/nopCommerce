using Nop.Core.Domain.Directory;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Directory
{
    /// <summary>
    /// Represents a measure dimension cache event consumer
    /// </summary>
    public partial class MeasureDimensionCacheEventConsumer : CacheEventConsumer<MeasureDimension>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(MeasureDimension entity)
        {
            RemoveByPrefix(NopDirectoryCachingDefaults.MeasureDimensionsPrefixCacheKey);
        }
    }
}

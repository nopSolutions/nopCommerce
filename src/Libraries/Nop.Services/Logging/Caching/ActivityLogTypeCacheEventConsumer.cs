using Nop.Core.Domain.Logging;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Logging.Caching
{
    /// <summary>
    /// Represents a activity log type cache event consumer
    /// </summary>
    public partial class ActivityLogTypeCacheEventConsumer : CacheEventConsumer<ActivityLogType>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCache(ActivityLogType entity)
        {
            await Remove(NopLoggingDefaults.ActivityTypeAllCacheKey);
        }
    }
}

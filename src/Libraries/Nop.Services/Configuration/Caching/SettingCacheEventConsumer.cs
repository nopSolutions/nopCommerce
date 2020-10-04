using Nop.Core.Domain.Configuration;
using Nop.Services.Caching;

namespace Nop.Services.Configuration.Caching
{
    /// <summary>
    /// Represents a setting cache event consumer
    /// </summary>
    public partial class SettingCacheEventConsumer : CacheEventConsumer<Setting>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override void ClearCache(Setting entity, EntityEventType entityEventType)
        {
            //clear setting cache in SettingService
        }
    }
}
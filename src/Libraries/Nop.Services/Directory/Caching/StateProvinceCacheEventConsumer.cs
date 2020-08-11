using Nop.Core.Caching;
using Nop.Core.Domain.Directory;
using Nop.Services.Caching;

namespace Nop.Services.Directory.Caching
{
    /// <summary>
    /// Represents a state province cache event consumer
    /// </summary>
    public partial class StateProvinceCacheEventConsumer : CacheEventConsumer<StateProvince>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override void ClearCache(StateProvince entity, EntityEventType entityEventType)
        {
            RemoveByPrefix(NopEntityCacheDefaults<StateProvince>.Prefix);
        }
    }
}

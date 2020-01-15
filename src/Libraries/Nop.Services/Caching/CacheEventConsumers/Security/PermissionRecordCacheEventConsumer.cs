using Nop.Core.Domain.Security;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Security
{
    /// <summary>
    /// Represents a permission record cache event consumer
    /// </summary>
    public partial class PermissionRecordCacheEventConsumer : CacheEventConsumer<PermissionRecord>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(PermissionRecord entity)
        {
            RemoveByPrefix(NopSecurityCachingDefaults.PermissionsPrefixCacheKey);
        }
    }
}

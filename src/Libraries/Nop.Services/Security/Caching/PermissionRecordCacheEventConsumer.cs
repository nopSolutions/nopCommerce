using Nop.Core.Domain.Security;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Security.Caching
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
        protected override async Task ClearCache(PermissionRecord entity)
        {
            await RemoveByPrefix(NopSecurityDefaults.PermissionAllowedPrefix, entity.SystemName);
        }
    }
}

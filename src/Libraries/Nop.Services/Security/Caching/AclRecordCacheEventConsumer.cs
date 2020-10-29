using Nop.Core.Domain.Security;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Security.Caching
{
    /// <summary>
    /// Represents a ACL record cache event consumer
    /// </summary>
    public partial class AclRecordCacheEventConsumer : CacheEventConsumer<AclRecord>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(AclRecord entity)
        {
            await RemoveAsync(NopSecurityDefaults.AclRecordCacheKey, entity.EntityId, entity.EntityName);
        }
    }
}

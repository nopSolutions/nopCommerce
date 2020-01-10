using Nop.Core.Domain.Security;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Security
{
    public partial class AclRecordCacheEventConsumer : CacheEventConsumer<AclRecord>
    {
        protected override void ClearCache(AclRecord entity)
        {
            RemoveByPrefix(NopSecurityCachingDefaults.AclRecordPrefixCacheKey);
        }
    }
}

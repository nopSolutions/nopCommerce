using Nop.Core.Domain.Security;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Security
{
    public partial class PermissionRecordCacheEventConsumer : CacheEventConsumer<PermissionRecord>
    {
        public override void ClearCashe(PermissionRecord entity)
        {
            _cacheManager.RemoveByPrefix(NopSecurityCachingDefaults.PermissionsPrefixCacheKey);
        }
    }
}

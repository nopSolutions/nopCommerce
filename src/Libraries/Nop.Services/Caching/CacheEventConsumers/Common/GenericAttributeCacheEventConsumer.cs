using Nop.Core.Domain.Common;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Common
{
    public partial class GenericAttributeCacheEventConsumer : EntityCacheEventConsumer<GenericAttribute>
    {
        public override void ClearCashe(GenericAttribute entity)
        {
            _cacheManager.RemoveByPrefix(NopCommonCachingDefaults.GenericAttributePrefixCacheKey);

            base.ClearCashe(entity);
        }
    }
}

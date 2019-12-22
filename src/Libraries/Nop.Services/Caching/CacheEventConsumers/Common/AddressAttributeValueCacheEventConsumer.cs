using Nop.Core.Domain.Common;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Common
{
    public partial class AddressAttributeValueCacheEventConsumer : CacheEventConsumer<AddressAttributeValue>
    {
        public override void ClearCashe(AddressAttributeValue entity)
        {
            _cacheManager.RemoveByPrefix(NopCommonCachingDefaults.AddressAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCommonCachingDefaults.AddressAttributeValuesPrefixCacheKey);
        }
    }
}

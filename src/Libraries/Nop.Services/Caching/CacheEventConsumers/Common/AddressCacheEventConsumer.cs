using Nop.Core.Domain.Common;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Common
{
    public partial class AddressCacheEventConsumer : CacheEventConsumer<Address>
    {
        public override void ClearCashe(Address entity)
        {
            _cacheManager.RemoveByPrefix(NopCommonCachingDefaults.AddressesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAddressesPrefixCacheKey);
        }
    }
}

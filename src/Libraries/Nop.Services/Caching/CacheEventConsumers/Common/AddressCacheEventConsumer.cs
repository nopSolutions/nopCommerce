using Nop.Core.Domain.Common;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Common
{
    public partial class AddressCacheEventConsumer : CacheEventConsumer<Address>
    {
        public override void ClearCache(Address entity)
        {
            RemoveByPrefix(NopCommonCachingDefaults.AddressesPrefixCacheKey);
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAddressesPrefixCacheKey, false);
        }
    }
}

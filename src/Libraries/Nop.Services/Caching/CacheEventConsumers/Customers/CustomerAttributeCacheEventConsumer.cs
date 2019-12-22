using Nop.Core.Domain.Customers;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    public partial class CustomerAttributeCacheEventConsumer : CacheEventConsumer<CustomerAttribute>
    {
        public override void ClearCashe(CustomerAttribute entity)
        {
            _cacheManager.RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAttributeValuesPrefixCacheKey);
        }
    }
}

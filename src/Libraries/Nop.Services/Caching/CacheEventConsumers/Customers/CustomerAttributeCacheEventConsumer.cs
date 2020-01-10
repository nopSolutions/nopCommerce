using Nop.Core.Domain.Customers;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    public partial class CustomerAttributeCacheEventConsumer : CacheEventConsumer<CustomerAttribute>
    {
        protected override void ClearCache(CustomerAttribute entity)
        {
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAttributesPrefixCacheKey);
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAttributeValuesPrefixCacheKey);
        }
    }
}

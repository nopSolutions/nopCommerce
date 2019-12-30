using Nop.Core.Domain.Customers;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    public partial class CustomerAttributeValueCacheEventConsumer : CacheEventConsumer<CustomerAttributeValue>
    {
        public override void ClearCache(CustomerAttributeValue entity)
        {
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAttributesPrefixCacheKey);
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAttributeValuesPrefixCacheKey);
        }
    }
}
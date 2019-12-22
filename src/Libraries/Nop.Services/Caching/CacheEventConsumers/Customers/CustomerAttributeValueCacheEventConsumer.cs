using Nop.Core.Domain.Customers;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    public partial class CustomerAttributeValueCacheEventConsumer : CacheEventConsumer<CustomerAttributeValue>
    {
        public override void ClearCashe(CustomerAttributeValue entity)
        {
            _cacheManager.RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAttributeValuesPrefixCacheKey);
        }
    }
}
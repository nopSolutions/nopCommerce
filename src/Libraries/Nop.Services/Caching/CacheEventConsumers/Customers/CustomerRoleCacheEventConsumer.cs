using Nop.Core.Domain.Customers;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    public partial class CustomerRoleCacheEventConsumer : CacheEventConsumer<CustomerRole>
    {
        protected override void ClearCache(CustomerRole entity)
        {
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerRolesPrefixCacheKey);
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerCustomerRolesPrefixCacheKey, false);
        }
    }
}

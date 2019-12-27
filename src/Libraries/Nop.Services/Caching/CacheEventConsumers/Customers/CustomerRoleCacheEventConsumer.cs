using Nop.Core.Domain.Customers;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    public partial class CustomerRoleCacheEventConsumer : CacheEventConsumer<CustomerRole>
    {
        public override void ClearCashe(CustomerRole entity)
        {
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerRolesPrefixCacheKey);
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerCustomerRolesPrefixCacheKey, false);
        }
    }
}

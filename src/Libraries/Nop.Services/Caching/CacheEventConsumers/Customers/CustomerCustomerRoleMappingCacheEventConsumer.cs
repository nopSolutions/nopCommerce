using Nop.Core.Domain.Customers;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    /// <summary>
    /// Represents a customer-customer role mapping class
    /// </summary>
    public partial class CustomerCustomerRoleMappingCacheEventConsumer : EntityCacheEventConsumer<CustomerCustomerRoleMapping>
    {
        public override void ClearCashe(CustomerCustomerRoleMapping entity)
        {
            _cacheManager.RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerRoleIdsPrefixCacheKey);
            base.ClearCashe(entity);
        }
    }
}
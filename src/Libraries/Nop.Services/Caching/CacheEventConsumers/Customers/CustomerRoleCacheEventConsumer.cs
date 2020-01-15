using Nop.Core.Domain.Customers;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    /// <summary>
    /// Represents a customer role cache event consumer
    /// </summary>
    public partial class CustomerRoleCacheEventConsumer : CacheEventConsumer<CustomerRole>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(CustomerRole entity)
        {
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerRolesPrefixCacheKey);
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerCustomerRolesPrefixCacheKey, false);
        }
    }
}

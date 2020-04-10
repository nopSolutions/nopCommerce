using Nop.Core.Domain.Customers;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    /// <summary>
    /// Represents a customer address mapping cache event consumer
    /// </summary>
    public partial class CustomerAddressMappingCacheEventConsumer : CacheEventConsumer<CustomerAddressMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(CustomerAddressMapping entity)
        {
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAddressesPrefixCacheKey, false);
        }
    }
}
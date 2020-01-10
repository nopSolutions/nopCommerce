using Nop.Core.Domain.Customers;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    /// <summary>
    /// Represents a customer-address mapping class
    /// </summary>
    public partial class CustomerAddressMappingCacheEventConsumer : CacheEventConsumer<CustomerAddressMapping>
    {
        protected override void ClearCache(CustomerAddressMapping entity)
        {
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAddressesPrefixCacheKey, false);
        }
    }
}
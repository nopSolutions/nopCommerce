using Nop.Core.Domain.Customers;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    /// <summary>
    /// Represents a customer-address mapping class
    /// </summary>
    public partial class CustomerAddressMappingCacheEventConsumer : EntityCacheEventConsumer<CustomerAddressMapping>
    {
        public override void ClearCashe(CustomerAddressMapping entity)
        {
            _cacheManager.RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAddressesPrefixCacheKey);
            base.ClearCashe(entity);
        }
    }
}
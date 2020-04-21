using Nop.Core.Domain.Customers;
using Nop.Services.Caching;

namespace Nop.Services.Customers.Caching
{
    /// <summary>
    /// Represents a customer attribute cache event consumer
    /// </summary>
    public partial class CustomerAttributeCacheEventConsumer : CacheEventConsumer<CustomerAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(CustomerAttribute entity)
        {
            Remove(NopCustomerServicesDefaults.CustomerAttributesAllCacheKey);
            Remove(_cacheKeyService.PrepareKey(NopCustomerServicesDefaults.CustomerAttributeValuesAllCacheKey, entity));
        }
    }
}

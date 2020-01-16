using Nop.Core.Domain.Customers;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    /// <summary>
    /// Represents a customer attribute value cache event consumer
    /// </summary>
    public partial class CustomerAttributeValueCacheEventConsumer : CacheEventConsumer<CustomerAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(CustomerAttributeValue entity)
        {
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAttributesPrefixCacheKey);
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAttributeValuesPrefixCacheKey);
        }
    }
}
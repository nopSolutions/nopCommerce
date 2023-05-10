using Nop.Core.Domain.Customers;
using Nop.Services.Attributes;
using Nop.Services.Caching;

namespace Nop.Services.Customers.Caching
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
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(CustomerAttributeValue entity)
        {
            await RemoveAsync(NopAttributeDefaults.AttributeValuesByAttributeCacheKey, nameof(CustomerAttribute), entity.AttributeId);
        }
    }
}
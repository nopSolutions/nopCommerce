using Nop.Core.Domain.Orders;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Orders.Caching
{
    /// <summary>
    /// Represents a checkout attribute value cache event consumer
    /// </summary>
    public partial class CheckoutAttributeValueCacheEventConsumer : CacheEventConsumer<CheckoutAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(CheckoutAttributeValue entity)
        {
            await RemoveAsync(NopOrderDefaults.CheckoutAttributeValuesAllCacheKey, entity.CheckoutAttributeId);
        }
    }
}

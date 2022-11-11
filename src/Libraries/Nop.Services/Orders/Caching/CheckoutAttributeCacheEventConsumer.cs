using Nop.Core.Domain.Orders;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Orders.Caching
{
    /// <summary>
    /// Represents a checkout attribute cache event consumer
    /// </summary>
    public partial class CheckoutAttributeCacheEventConsumer : CacheEventConsumer<CheckoutAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(CheckoutAttribute entity)
        {
            await RemoveAsync(NopOrderDefaults.CheckoutAttributeValuesAllCacheKey, entity);
        }
    }
}

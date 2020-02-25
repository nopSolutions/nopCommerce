using Nop.Core.Domain.Orders;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
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
        protected override void ClearCache(CheckoutAttribute entity)
        {
            RemoveByPrefix(NopOrderCachingDefaults.CheckoutAttributesPrefixCacheKey);
            RemoveByPrefix(NopOrderCachingDefaults.CheckoutAttributeValuesPrefixCacheKey);
        }
    }
}

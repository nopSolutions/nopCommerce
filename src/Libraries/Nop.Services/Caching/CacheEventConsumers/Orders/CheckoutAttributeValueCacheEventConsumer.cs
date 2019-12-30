using Nop.Core.Domain.Orders;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    public partial class CheckoutAttributeValueCacheEventConsumer : CacheEventConsumer<CheckoutAttributeValue>
    {
        public override void ClearCache(CheckoutAttributeValue entity)
        {
            RemoveByPrefix(NopOrderCachingDefaults.CheckoutAttributesPrefixCacheKey);
            RemoveByPrefix(NopOrderCachingDefaults.CheckoutAttributeValuesPrefixCacheKey);
        }
    }
}

using Nop.Core.Domain.Orders;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    public partial class CheckoutAttributeValueCacheEventConsumer : CacheEventConsumer<CheckoutAttributeValue>
    {
        protected override void ClearCache(CheckoutAttributeValue entity)
        {
            RemoveByPrefix(NopNewsCachingDefaults.CheckoutAttributesPrefixCacheKey);
            RemoveByPrefix(NopNewsCachingDefaults.CheckoutAttributeValuesPrefixCacheKey);
        }
    }
}

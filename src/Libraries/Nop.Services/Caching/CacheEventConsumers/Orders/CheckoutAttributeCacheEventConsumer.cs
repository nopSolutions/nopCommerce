using Nop.Core.Domain.Orders;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    public partial class CheckoutAttributeCacheEventConsumer : CacheEventConsumer<CheckoutAttribute>
    {
        protected override void ClearCache(CheckoutAttribute entity)
        {
            RemoveByPrefix(NopNewsCachingDefaults.CheckoutAttributesPrefixCacheKey);
            RemoveByPrefix(NopNewsCachingDefaults.CheckoutAttributeValuesPrefixCacheKey);
        }
    }
}

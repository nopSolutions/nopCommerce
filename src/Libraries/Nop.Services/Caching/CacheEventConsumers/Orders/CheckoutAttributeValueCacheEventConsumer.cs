using Nop.Core.Domain.Orders;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    public partial class CheckoutAttributeValueCacheEventConsumer : CacheEventConsumer<CheckoutAttributeValue>
    {
        public override void ClearCashe(CheckoutAttributeValue entity)
        {
            _cacheManager.RemoveByPrefix(NopOrderCachingDefaults.CheckoutAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopOrderCachingDefaults.CheckoutAttributeValuesPrefixCacheKey);
        }
    }
}

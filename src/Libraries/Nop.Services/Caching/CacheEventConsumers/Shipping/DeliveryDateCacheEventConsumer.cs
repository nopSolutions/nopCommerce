using Nop.Core.Domain.Shipping;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Shipping
{
    /// <summary>
    /// Represents a delivery date cache event consumer
    /// </summary>
    public partial class DeliveryDateCacheEventConsumer : CacheEventConsumer<DeliveryDate>
    {
        protected override void ClearCache(DeliveryDate entity)
        {
            RemoveByPrefix(NopShippingCachingDefaults.DeliveryDatesPrefixCacheKey);
        }
    }
}
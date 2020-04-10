using Nop.Core.Domain.Shipping;
using Nop.Services.Caching;

namespace Nop.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a shipping method cache event consumer
    /// </summary>
    public partial class ShippingMethodCacheEventConsumer : CacheEventConsumer<ShippingMethod>
    {
        protected override void ClearCache(ShippingMethod entity)
        {
            RemoveByPrefix(NopShippingDefaults.ShippingMethodsAllPrefixCacheKey);
        }
    }
}

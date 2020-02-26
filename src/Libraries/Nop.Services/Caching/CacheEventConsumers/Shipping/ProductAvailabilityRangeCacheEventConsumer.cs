using Nop.Core.Domain.Shipping;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Shipping
{
    /// <summary>
    /// Represents a product availability range cache event consumer
    /// </summary>
    public partial class ProductAvailabilityRangeCacheEventConsumer : CacheEventConsumer<ProductAvailabilityRange>
    {
        protected override void ClearCache(ProductAvailabilityRange entity)
        {
            RemoveByPrefix(NopShippingCachingDefaults.ProductAvailabilityPrefixCacheKey);
        }
    }
}
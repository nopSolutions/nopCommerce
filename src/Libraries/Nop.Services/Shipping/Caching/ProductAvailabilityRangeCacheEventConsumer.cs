using Nop.Core.Domain.Shipping;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a product availability range cache event consumer
    /// </summary>
    public partial class ProductAvailabilityRangeCacheEventConsumer : CacheEventConsumer<ProductAvailabilityRange>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCache(ProductAvailabilityRange entity)
        {
            await Remove(NopShippingDefaults.ProductAvailabilityAllCacheKey);
        }
    }
}
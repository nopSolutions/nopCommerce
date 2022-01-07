using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tier price cache event consumer
    /// </summary>
    public partial class TierPriceCacheEventConsumer : CacheEventConsumer<TierPrice>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(TierPrice entity)
        {
            await RemoveAsync(NopCatalogDefaults.TierPricesByProductCacheKey, entity.ProductId);
            await RemoveByPrefixAsync(NopCatalogDefaults.ProductPricePrefix, entity.ProductId);
        }
    }
}

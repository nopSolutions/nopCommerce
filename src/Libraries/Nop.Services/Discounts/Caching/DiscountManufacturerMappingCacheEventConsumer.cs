using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Caching;

namespace Nop.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount-manufacturer mapping cache event consumer
    /// </summary>
    public partial class DiscountManufacturerMappingCacheEventConsumer : CacheEventConsumer<DiscountManufacturerMapping>
    {
        protected override async Task ClearCacheAsync(DiscountManufacturerMapping entity)
        {
            await RemoveAsync(NopDiscountDefaults.AppliedDiscountsCacheKey, nameof(Manufacturer), entity.EntityId);

            await base.ClearCacheAsync(entity);
        }
    }
}
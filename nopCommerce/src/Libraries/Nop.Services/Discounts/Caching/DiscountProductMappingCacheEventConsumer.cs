using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Caching;

namespace Nop.Services.Discounts.Caching;

/// <summary>
/// Represents a discount-product mapping cache event consumer
/// </summary>
public partial class DiscountProductMappingCacheEventConsumer : CacheEventConsumer<DiscountProductMapping>
{
    protected override async Task ClearCacheAsync(DiscountProductMapping entity)
    {
        await RemoveAsync(NopDiscountDefaults.AppliedDiscountsCacheKey, nameof(Product), entity.EntityId);

        await base.ClearCacheAsync(entity);
    }
}
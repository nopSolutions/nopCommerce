using Nop.Core.Domain.PriceLists;
using Nop.Services.Caching;

namespace Nop.Services.PriceLists.Caching;

/// <summary>
/// Represents a price list cache event consumer
/// </summary>
public partial class PriceListCacheEventConsumer : CacheEventConsumer<PriceList>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(PriceList entity)
    {
        await RemoveByPrefixAsync(NopPriceListDefaults.PriceListPrefix);
    }
}

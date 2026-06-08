using Nop.Core.Domain.PriceLists;
using Nop.Services.Caching;

namespace Nop.Services.PriceLists.Caching;

/// <summary>
/// Represents a price list customer cache event consumer
/// </summary>
public partial class PriceListCustomerCacheEventConsumer : CacheEventConsumer<PriceListCustomer>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(PriceListCustomer entity)
    {
        await RemoveByPrefixAsync(NopPriceListDefaults.PriceListPrefix);
    }
}

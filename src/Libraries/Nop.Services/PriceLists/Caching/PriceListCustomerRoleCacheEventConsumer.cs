using Nop.Core.Domain.PriceLists;
using Nop.Services.Caching;

namespace Nop.Services.PriceLists.Caching;

/// <summary>
/// Represents a price list customer role cache event consumer
/// </summary>
public partial class  PriceListCustomerRoleCacheEventConsumer : CacheEventConsumer<PriceListCustomerRole>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(PriceListCustomerRole entity)
    {
        await RemoveByPrefixAsync(NopPriceListDefaults.PriceListPrefix);
    }
}

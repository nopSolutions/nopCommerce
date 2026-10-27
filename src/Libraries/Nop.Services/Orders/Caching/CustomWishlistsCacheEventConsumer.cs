using Nop.Core.Domain.Orders;
using Nop.Services.Caching;

namespace Nop.Services.Orders.Caching;

/// <summary>
/// Represents a custom wishlists cache event consumer
/// </summary>
public partial class CustomWishlistsCacheEventConsumer : CacheEventConsumer<CustomWishlist>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(CustomWishlist entity)
    {
        await RemoveAsync(NopOrderDefaults.CustomWishlistCacheKey, entity.CustomerId);
        await base.ClearCacheAsync(entity);
    }
}
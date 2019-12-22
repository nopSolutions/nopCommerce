using Nop.Core.Domain.Orders;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a gift card usage history entry
    /// </summary>
    public partial class GiftCardUsageHistoryCacheEventConsumer : EntityCacheEventConsumer<GiftCardUsageHistory>
    {
    }
}

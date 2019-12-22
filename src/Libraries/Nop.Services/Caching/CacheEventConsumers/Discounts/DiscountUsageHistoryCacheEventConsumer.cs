using Nop.Core.Domain.Discounts;

namespace Nop.Services.Caching.CacheEventConsumers.Discounts
{
    /// <summary>
    /// Represents a discount usage history entry
    /// </summary>
    public partial class DiscountUsageHistoryCacheEventConsumer : EntityCacheEventConsumer<DiscountUsageHistory>
    {
    }
}

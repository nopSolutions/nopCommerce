using Nop.Core.Domain.Discounts;
using Nop.Services.Caching;

namespace Nop.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount usage history cache event consumer
    /// </summary>
    public partial class DiscountUsageHistoryCacheEventConsumer : CacheEventConsumer<DiscountUsageHistory>
    {
    }
}

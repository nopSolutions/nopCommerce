using Nop.Core.Domain.Orders;
using Nop.Services.Caching;

namespace Nop.Services.Orders.Caching;

/// <summary>
/// Represents a gift card usage history cache event consumer
/// </summary>
public partial class GiftCardUsageHistoryCacheEventConsumer : CacheEventConsumer<GiftCardUsageHistory>
{
}
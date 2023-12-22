using Nop.Core.Domain.Orders;
using Nop.Services.Caching;

namespace Nop.Services.Orders.Caching;

/// <summary>
/// Represents a order cache event consumer
/// </summary>
public partial class OrderCacheEventConsumer : CacheEventConsumer<Order>
{
}
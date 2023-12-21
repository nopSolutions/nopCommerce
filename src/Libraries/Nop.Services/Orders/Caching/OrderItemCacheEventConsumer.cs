using Nop.Core.Domain.Orders;
using Nop.Services.Caching;

namespace Nop.Services.Orders.Caching;

/// <summary>
/// Represents an order item cache event consumer
/// </summary>
public partial class OrderItemCacheEventConsumer : CacheEventConsumer<OrderItem>
{
}
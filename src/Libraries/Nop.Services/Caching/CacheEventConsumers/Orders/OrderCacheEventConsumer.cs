using Nop.Core.Domain.Orders;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a order cache event consumer
    /// </summary>
    public partial class OrderCacheEventConsumer : CacheEventConsumer<Order>
    {
    }
}
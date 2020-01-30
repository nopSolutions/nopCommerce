using Nop.Core.Domain.Shipping;

namespace Nop.Services.Caching.CacheEventConsumers.Shipping
{
    /// <summary>
    /// Represents a shipment item cache event consumer
    /// </summary>
    public partial class ShipmentItemCacheEventConsumer : CacheEventConsumer<ShipmentItem>
    {
    }
}
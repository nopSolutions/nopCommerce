using Nop.Core.Domain.Shipping;

namespace Nop.Services.Caching.CacheEventConsumers.Shipping
{
    /// <summary>
    /// Represents a shipment item
    /// </summary>
    public partial class ShipmentItemCacheEventConsumer : EntityCacheEventConsumer<ShipmentItem>
    {
    }
}
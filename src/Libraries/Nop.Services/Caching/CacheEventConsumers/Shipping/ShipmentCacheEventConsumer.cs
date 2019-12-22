using Nop.Core.Domain.Shipping;

namespace Nop.Services.Caching.CacheEventConsumers.Shipping
{
    /// <summary>
    /// Represents a shipment
    /// </summary>
    public partial class ShipmentCacheEventConsumer : EntityCacheEventConsumer<Shipment>
    {
    }
}
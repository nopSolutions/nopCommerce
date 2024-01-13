using Nop.Core.Domain.Shipping;
using Nop.Services.Caching;

namespace Nop.Services.Shipping.Caching;

/// <summary>
/// Represents a shipment item cache event consumer
/// </summary>
public partial class ShipmentItemCacheEventConsumer : CacheEventConsumer<ShipmentItem>
{
}
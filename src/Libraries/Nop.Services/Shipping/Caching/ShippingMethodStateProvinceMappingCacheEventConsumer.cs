using Nop.Core.Domain.Shipping;
using Nop.Services.Caching;

namespace Nop.Services.Shipping.Caching;

/// <summary>
/// Represents a shipping method-state/province mapping cache event consumer
/// </summary>
public partial class ShippingMethodStateProvinceMappingCacheEventConsumer : CacheEventConsumer<ShippingMethodStateProvinceMapping>
{
}
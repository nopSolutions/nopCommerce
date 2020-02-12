using Nop.Core.Domain.Shipping;

namespace Nop.Services.Caching.CacheEventConsumers.Shipping
{
    /// <summary>
    /// Represents a shipping method-country mapping cache event consumer
    /// </summary>
    public partial class ShippingMethodCountryMappingCacheEventConsumer : CacheEventConsumer<ShippingMethodCountryMapping>
    {
    }
}
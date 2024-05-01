using Nop.Core.Caching;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Shipping;

/// <summary>
/// Represents default values related to shipping services
/// </summary>
public static partial class NopShippingDefaults
{
    #region Caching defaults

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : country identifier
    /// {1} : state province identifier
    /// </remarks>
    public static CacheKey ShippingMethodsAllCacheKey => new("Nop.shippingmethod.all.{0}.{1}", NopEntityCacheDefaults<ShippingMethod>.AllPrefix);

    #endregion
}
using Nop.Core.Caching;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal;

/// <summary>
/// Represents constants of the "Fixed or by weight" shipping plugin
/// </summary>
public static class FixedByWeightByTotalDefaults
{
    /// <summary>
    /// The key of the settings to save fixed rate of the shipping method
    /// </summary>
    public const string FIXED_RATE_SETTINGS_KEY = "ShippingRateComputationMethod.FixedByWeightByTotal.Rate.ShippingMethodId{0}";

    /// <summary>
    /// The key of the settings to save transit days of the shipping method
    /// </summary>
    public const string TRANSIT_DAYS_SETTINGS_KEY = "ShippingRateComputationMethod.FixedByWeightByTotal.TransitDays.ShippingMethodId{0}";


    /// <summary>
    /// Gets a key for caching all entities
    /// 0 - Shipping method identifier
    /// 1 - Store identifier
    /// 2 - Warehouse identifier
    /// 3 - Country identifier
    /// 4 - State identifier
    /// 5 - Zip postal code
    /// </summary>
    public static CacheKey ShippingByWeightByTotalCacheKey =>
        new("Nop.shippingbyweightbytotal.{0}-{1}-{2}-{3}-{4}-{5}", ShippingByWeightByTotalCachePrefix);

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string ShippingByWeightByTotalCachePrefix => "Nop.shippingbyweightbytotal.";
}
using Nop.Core.Caching;

namespace Nop.Services.PriceLists;

/// <summary>
/// Represents default values related to price list services
/// </summary>
public static partial class NopPriceListDefaults
{
    #region Caching defaults

    /// <summary>
    /// Gets a key for caching all price lists
    /// </summary>
    /// <remarks>
    /// {0} : customer role IDs
    /// {1} : customer IDs
    /// {2} : is active
    /// </remarks>
    public static CacheKey PriceListAllCacheKey => new("Nop.pricelist.all.{0}-{1}-{2}");

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string PriceListPrefix => "Nop.pricelist.";

    #endregion
}

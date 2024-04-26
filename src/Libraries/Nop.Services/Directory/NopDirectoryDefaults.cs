using Nop.Core.Caching;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Directory;

/// <summary>
/// Represents default values related to directory services
/// </summary>
public static partial class NopDirectoryDefaults
{
    #region Caching defaults

    #region Countries

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : Two letter ISO code
    /// </remarks>
    public static CacheKey CountriesByTwoLetterCodeCacheKey => new("Nop.country.bytwoletter.{0}", NopEntityCacheDefaults<Country>.Prefix);

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : Two letter ISO code
    /// </remarks>
    public static CacheKey CountriesByThreeLetterCodeCacheKey => new("Nop.country.bythreeletter.{0}", NopEntityCacheDefaults<Country>.Prefix);

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : language ID
    /// {1} : show hidden records?
    /// {2} : current store ID
    /// </remarks>
    public static CacheKey CountriesAllCacheKey => new("Nop.country.all.{0}-{1}-{2}", NopEntityCacheDefaults<Country>.Prefix);

    #endregion

    #region Currencies

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : show hidden records?
    /// </remarks>
    public static CacheKey CurrenciesAllCacheKey => new("Nop.currency.all.{0}", NopEntityCacheDefaults<Currency>.AllPrefix);

    #endregion

    #region States and provinces

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : country ID
    /// {1} : language ID
    /// {2} : show hidden records?
    /// </remarks>
    public static CacheKey StateProvincesByCountryCacheKey => new("Nop.stateprovince.bycountry.{0}-{1}-{2}", NopEntityCacheDefaults<StateProvince>.Prefix);

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : show hidden records?
    /// </remarks>
    public static CacheKey StateProvincesAllCacheKey => new("Nop.stateprovince.all.{0}", NopEntityCacheDefaults<StateProvince>.Prefix);

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : abbreviation
    /// {1} : country ID
    /// </remarks>
    public static CacheKey StateProvincesByAbbreviationCacheKey => new("Nop.stateprovince.byabbreviation.{0}-{1}", NopEntityCacheDefaults<StateProvince>.Prefix);

    #endregion

    #endregion
}
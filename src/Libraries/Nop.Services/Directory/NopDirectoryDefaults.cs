using Nop.Core.Caching;

namespace Nop.Services.Directory
{
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
        public static CacheKey CountriesByTwoLetterCodeCacheKey => new CacheKey("Nop.country.twoletter-{0}", CountriesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Two letter ISO code
        /// </remarks>
        public static CacheKey CountriesByThreeLetterCodeCacheKey => new CacheKey("Nop.country.threeletter-{0}", CountriesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : show hidden records?
        /// </remarks>
        public static CacheKey CountriesAllCacheKey => new CacheKey("Nop.country.all-{0}-{1}", CountriesPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CountriesPrefixCacheKey => "Nop.country.";

        #endregion

        #region Currencies

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey CurrenciesAllCacheKey => new CacheKey("Nop.currency.all-{0}", CurrenciesAllPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CurrenciesAllPrefixCacheKey => "Nop.currency.all";

        #endregion

        #region Measures

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey MeasureDimensionsAllCacheKey => new CacheKey("Nop.measuredimension.all");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey MeasureWeightsAllCacheKey => new CacheKey("Nop.measureweight.all");

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
        public static CacheKey StateProvincesByCountryCacheKey => new CacheKey("Nop.stateprovince.all-{0}-{1}-{2}", StateProvincesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey StateProvincesAllCacheKey => new CacheKey("Nop.stateprovince.all-{0}", StateProvincesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : abbreviation
        /// {1} : country ID
        /// </remarks>
        public static CacheKey StateProvincesByAbbreviationCacheKey => new CacheKey("Nop.stateprovince.abbreviationcountryid-{0}-{1}", StateProvincesPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string StateProvincesPrefixCacheKey => "Nop.stateprovince.";

        #endregion

        #endregion
    }
}
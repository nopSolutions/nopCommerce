using Nop.Core.Caching;

namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to localization services
    /// </summary>
    public static partial class NopLocalizationCachingDefaults
    {
        #region Languages

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : show hidden records?
        /// </remarks>
        public static CacheKey LanguagesAllCacheKey => new CacheKey("Nop.language.all-{0}-{1}", LanguagesByStoreIdPrefixCacheKey, LanguagesAllPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        public static string LanguagesByStoreIdPrefixCacheKey => "Nop.language.all-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string LanguagesAllPrefixCacheKey => "Nop.language.all";

        #endregion

        #region Locales

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocaleStringResourcesAllPublicCacheKey => new CacheKey("Nop.lsr.all.public-{0}", LocaleStringResourcesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocaleStringResourcesAllAdminCacheKey => new CacheKey("Nop.lsr.all.admin-{0}", LocaleStringResourcesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocaleStringResourcesAllCacheKey => new CacheKey("Nop.lsr.all-{0}", LocaleStringResourcesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : resource key
        /// </remarks>
        public static CacheKey LocaleStringResourcesByResourceNameCacheKey => new CacheKey("Nop.lsr.{0}-{1}", LocaleStringResourcesByResourceNamePrefixCacheKey, LocaleStringResourcesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string LocaleStringResourcesByResourceNamePrefixCacheKey => "Nop.lsr.{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string LocaleStringResourcesPrefixCacheKey => "Nop.lsr.";

        #endregion

        #region Localized properties

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : entity ID
        /// {2} : locale key group
        /// {3} : locale key
        /// </remarks>
        public static CacheKey LocalizedPropertyCacheKey => new CacheKey("Nop.localizedproperty.value-{0}-{1}-{2}-{3}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey LocalizedPropertyAllCacheKey => new CacheKey("Nop.localizedproperty.all");
        
        #endregion
    }
}
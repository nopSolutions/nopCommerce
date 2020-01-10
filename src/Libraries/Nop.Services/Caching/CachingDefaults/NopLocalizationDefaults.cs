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
        /// {0} : language ID
        /// </remarks>
        public static string LanguagesByIdCacheKey => "Nop.language.id-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : show hidden records?
        /// </remarks>
        public static string LanguagesAllCacheKey => "Nop.language.all-{0}-{1}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string LanguagesPrefixCacheKey => "Nop.language.";

        #endregion

        #region Locales

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string LocaleStringResourcesAllPublicCacheKey => "Nop.lsr.all.public-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string LocaleStringResourcesAllCacheKey => "Nop.lsr.all-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string LocaleStringResourcesAllAdminCacheKey => "Nop.lsr.all.admin-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : resource key
        /// </remarks>
        public static string LocaleStringResourcesByResourceNameCacheKey => "Nop.lsr.{0}-{1}";

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
        public static string LocalizedPropertyCacheKey => "Nop.localizedproperty.value-{0}-{1}-{2}-{3}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string LocalizedPropertyAllCacheKey => "Nop.localizedproperty.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string LocalizedPropertyPrefixCacheKey => "Nop.localizedproperty.";

        #endregion
    }
}
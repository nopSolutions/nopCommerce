using Nop.Core;
using Nop.Core.Caching;

namespace Nop.Plugin.Tax.Avalara
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public class AvalaraTaxDefaults
    {
        /// <summary>
        /// Gets the Avalara tax provider system name
        /// </summary>
        public static string SystemName => "Tax.Avalara";

        /// <summary>
        /// Gets the Avalara tax provider connector name
        /// </summary>
        public static string ApplicationName => "nopCommerce-AvalaraTaxRateProvider|a0o33000004BoPM";

        /// <summary>
        /// Gets the Avalara tax provider version (used a nopCommerce version here)
        /// </summary>
        public static string ApplicationVersion => $"v{NopVersion.CurrentVersion}";

        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string ConfigurationRouteName => "Plugin.Tax.Avalara.Configure";

        /// <summary>
        /// Gets the name of the generic attribute that is used to store Avalara system tax code description
        /// </summary>
        public static string TaxCodeDescriptionAttribute => "AvalaraTaxCodeDescription";

        /// <summary>
        /// Gets the name of the generic attribute that is used to store a tax code type
        /// </summary>
        public static string TaxCodeTypeAttribute => "AvalaraTaxCodeType";

        /// <summary>
        /// Gets the name of the generic attribute that is used to store an entity use code (customer usage type)
        /// </summary>
        public static string EntityUseCodeAttribute => "AvalaraEntityUseCode";

        /// <summary>
        /// Gets the key for caching tax rate
        /// </summary>
        /// <remarks>
        /// {0} - Customer id
        /// {1} - Tax category
        /// {2} - Product SKU
        /// {3} - Address
        /// {4} - City
        /// {5} - State or province identifier
        /// {6} - Country identifier
        /// {7} - Zip postal code
        /// </remarks>
        public static CacheKey TaxRateCacheKey => new CacheKey("Nop.avalara.taxrate-{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}");

        /// <summary>
        /// Gets the key for caching Avalara tax code types
        /// </summary>
        public static CacheKey TaxCodeTypesCacheKey => new CacheKey("Nop.avalara.taxcodetypes");

        /// <summary>
        /// Gets the key for caching Avalara system entity use codes
        /// </summary>
        public static CacheKey EntityUseCodesCacheKey => new CacheKey("Nop.avalara.entityusecodes");

        /// <summary>
        /// Gets the name of the view component to display entity use code field
        /// </summary>
        public const string ENTITY_USE_CODE_VIEW_COMPONENT_NAME = "AvalaraEntityUseCode";

        /// <summary>
        /// Gets the name of the view component to display export items button
        /// </summary>
        public const string EXPORT_ITEMS_VIEW_COMPONENT_NAME = "AvalaraExportItems";

        /// <summary>
        /// Gets the name of the view component to validate entered address
        /// </summary>
        public const string ADDRESS_VALIDATION_VIEW_COMPONENT_NAME = "AvalaraAddressValidation";

        /// <summary>
        /// Gets the generic attribute name to hide general settings block on the plugin configuration page
        /// </summary>
        public static string HideGeneralBlock => "AvalaraPage.HideGeneralBlock";

        /// <summary>
        /// Gets the generic attribute name to hide log block on the plugin configuration page
        /// </summary>
        public static string HideLogBlock => "AvalaraPage.HideLogBlock";
    }
}
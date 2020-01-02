using Nop.Core;

namespace Nop.Plugin.Tax.Avalara
{
    /// <summary>
    /// Represents constants of the Avalara tax provider
    /// </summary>
    public class AvalaraTaxDefaults
    {
        /// <summary>
        /// Avalara tax provider system name
        /// </summary>
        public static string SystemName => "Tax.Avalara";

        /// <summary>
        /// Avalara tax provider connector name
        /// </summary>
        public static string ApplicationName => "nopCommerce-AvalaraTaxRateProvider|a0o33000004BoPM";

        /// <summary>
        /// Avalara tax provider version (used a nopCommerce version here)
        /// </summary>
        public static string ApplicationVersion => $"v{NopVersion.CurrentVersion}";

        /// <summary>
        /// Name of the generic attribute that is used to store Avalara system tax code description
        /// </summary>
        public static string TaxCodeDescriptionAttribute => "AvalaraTaxCodeDescription";

        /// <summary>
        /// Name of the generic attribute that is used to store a tax code type
        /// </summary>
        public static string TaxCodeTypeAttribute => "AvalaraTaxCodeType";

        /// <summary>
        /// Name of the generic attribute that is used to store an entity use code (customer usage type)
        /// </summary>
        public static string EntityUseCodeAttribute => "AvalaraEntityUseCode";

        /// <summary>
        /// Name of a session custom value to store tax details received from the Avalara
        /// </summary>
        public static string TaxDetailsSessionValue => "AvalaraTaxDetails";

        /// <summary>
        /// Name of the field to specify the tax origin address type
        /// </summary>
        public static string TaxOriginField => "AvalaraTaxOriginAddressType";

        /// <summary>
        /// Key for caching tax rate for the specified address
        /// </summary>
        /// <remarks>
        /// {0} - Address
        /// {1} - City
        /// {2} - State or province identifier
        /// {3} - Country identifier
        /// {4} - Zip postal code
        /// </remarks>
        public static string TaxRateCacheKey => "Nop.avalara.taxrate.address-{0}-{1}-{2}-{3}-{4}";

        /// <summary>
        /// Key for caching Avalara tax code types
        /// </summary>
        public static string TaxCodeTypesCacheKey => "Nop.avalara.taxcodetypes";

        /// <summary>
        /// Key for caching Avalara system entity use codes
        /// </summary>
        public static string EntityUseCodesCacheKey => "Nop.avalara.entityusecodes";

        /// <summary>
        /// Name of the view component to display entity use code field
        /// </summary>
        public const string ENTITY_USE_CODE_VIEW_COMPONENT_NAME = "AvalaraEntityUseCode";

        /// <summary>
        /// Name of the view component to display tax origin address type field
        /// </summary>
        public const string TAX_ORIGIN_VIEW_COMPONENT_NAME = "AvalaraTaxOrigin";

        /// <summary>
        /// Name of the view component to display export items button
        /// </summary>
        public const string EXPORT_ITEMS_VIEW_COMPONENT_NAME = "AvalaraExportItems";

        /// <summary>
        /// Name of the view component to display entity use code
        /// </summary>
        public const string TAX_CODES_VIEW_COMPONENT_NAME = "AvalaraTaxCodes";

        /// <summary>
        /// Name of the view component to validate entered address
        /// </summary>
        public const string ADDRESS_VALIDATION_VIEW_COMPONENT_NAME = "AvalaraAddressValidation";

        /// <summary>
        /// Custom data objects context name
        /// </summary>
        public static string ObjectContextName = "nop_object_context_tax_avalara";

        /// <summary>
        /// Generic attribute name to hide general settings block on the plugin configuration page
        /// </summary>
        public static string HideGeneralBlock = "AvalaraPage.HideGeneralBlock";

        /// <summary>
        /// Generic attribute name to hide log block on the plugin configuration page
        /// </summary>
        public static string HideLogBlock = "AvalaraPage.HideLogBlock";
    }
}
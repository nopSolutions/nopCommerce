namespace Nop.Services.Vendors
{
    /// <summary>
    /// Represents default values related to vendor services
    /// </summary>
    public static partial class NopVendorsServiceDefaults
    {
        /// <summary>
        /// Gets a key for caching all vendor attributes
        /// </summary>
        public static string VendorAttributesAllCacheKey => "Nop.vendorattribute.all";

        /// <summary>
        /// Gets a key for caching a vendor attribute
        /// </summary>
        /// <remarks>
        /// {0} : vendor attribute ID
        /// </remarks>
        public static string VendorAttributesByIdCacheKey => "Nop.vendorattribute.id-{0}";

        /// <summary>
        /// Gets a key for caching vendor attribute values of the vendor attribute
        /// </summary>
        /// <remarks>
        /// {0} : vendor attribute ID
        /// </remarks>
        public static string VendorAttributeValuesAllCacheKey => "Nop.vendorattributevalue.all-{0}";

        /// <summary>
        /// Gets a key for caching a vendor attribute value
        /// </summary>
        /// <remarks>
        /// {0} : vendor attribute value ID
        /// </remarks>
        public static string VendorAttributeValuesByIdCacheKey => "Nop.vendorattributevalue.id-{0}";

        /// <summary>
        /// Gets a key pattern to clear cached vendor attributes
        /// </summary>
        public static string VendorAttributesPrefixCacheKey => "Nop.vendorattribute.";

        /// <summary>
        /// Gets a key pattern to clear cached vendor attribute values
        /// </summary>
        public static string VendorAttributeValuesPrefixCacheKey => "Nop.vendorattributevalue.";
    }
}
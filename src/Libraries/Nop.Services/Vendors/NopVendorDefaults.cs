using Nop.Core.Caching;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Represents default values related to vendor services
    /// </summary>
    public static partial class NopVendorDefaults
    {
        /// <summary>
        /// Gets a generic attribute key to store vendor additional info
        /// </summary>
        public static string VendorAttributes => "VendorAttributes";

        /// <summary>
        /// Gets default prefix for vendor
        /// </summary>
        public static string VendorAttributePrefix => "vendor_attribute_";

        #region Caching defaults

        /// <summary>
        /// Gets a key for caching vendor attribute values of the vendor attribute
        /// </summary>
        /// <remarks>
        /// {0} : vendor attribute ID
        /// </remarks>
        public static CacheKey VendorAttributeValuesByAttributeCacheKey => new("Nop.vendorattributevalue.byattribute.{0}");

        #endregion
    }
}
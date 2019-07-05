namespace Nop.Services.Tax
{
    /// <summary>
    /// Represents default values related to tax services
    /// </summary>
    public static partial class NopTaxDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string TaxCategoriesAllCacheKey => "Nop.taxcategory.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tax category ID
        /// </remarks>
        public static string TaxCategoriesByIdCacheKey => "Nop.taxcategory.id-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string TaxCategoriesPrefixCacheKey => "Nop.taxcategory.";
        
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : address ID
        /// </remarks>
        public static string TaxAddressByAddressIdCacheKey => "Nop.taxaddress.address.id-{0}-";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string TaxAddressPrefixCacheKey => "Nop.taxaddress.address.id-{0}-";
    }
}
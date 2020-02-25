namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to tax services
    /// </summary>
    public static partial class NopTaxCachingDefaults
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
    }
}
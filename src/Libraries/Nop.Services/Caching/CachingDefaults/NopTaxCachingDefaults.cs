using Nop.Core.Caching;

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
        public static CacheKey TaxCategoriesAllCacheKey => new CacheKey("Nop.taxcategory.all");
    }
}
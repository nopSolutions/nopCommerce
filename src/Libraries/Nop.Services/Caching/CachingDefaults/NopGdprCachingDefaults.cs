namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to Gdpr services
    /// </summary>
    public static partial class NopGdprCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string ConsentsAllCacheKey => "Nop.consents.all";
       
        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ConsentsPrefixCacheKey => "Nop.consents.";
    }
}
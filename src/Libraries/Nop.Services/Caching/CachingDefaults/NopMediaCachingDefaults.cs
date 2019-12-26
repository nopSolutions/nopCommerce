namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to media services
    /// </summary>
    public static partial class NopMediaCachingDefaults
    {
        /// <summary>
        /// Gets a key to cache whether thumb exists
        /// </summary>
        /// <remarks>
        /// {0} : thumb file name
        /// </remarks>
        public static string ThumbExistsCacheKey => "Nop.azure.thumb.exists-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ThumbsPrefixCacheKey => "Nop.azure.thumb";

        /// <summary>
        /// Gets a key to cache
        /// </summary>
        /// <remarks>
        /// {0} : virtual path
        /// </remarks>
        public static string PicturesByVirtualPathCacheKey => "Nop.pictures.virtualpath-{0}";
        
        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string PicturesPrefixCacheKey => "Nop.pictures";
    }
}
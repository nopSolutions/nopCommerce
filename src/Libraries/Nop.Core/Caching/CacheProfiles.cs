namespace Nop.Core.Caching
{
    /// <summary>
    /// A class that defines constant strings that represent that known cache profiles 
    /// to define timeout minutes of keys. Use these constants rather than 
    /// using inline values for cache manager.
    /// </summary>
    public partial class CacheProfiles
    {
        /// <summary>
        /// The default profile for the application.
        /// profile
        /// </summary>
        public const int DefaultProfile = 60;

        /// <summary>
        /// A profile that quickly expires.
        /// </summary>
        public const int ShortLivedProfile = 2;

        /// <summary>
        /// A profile that lives for a longer period of time.
        /// </summary>
        public const int LongLivedProfile = 720;

        /// <summary>
        /// A short lived cache profile for caching customer specific content.
        /// </summary>
        public const int CustomerCacheProfile = 1;

        /// <summary>
        /// A short lived cache profile for caching customer specific content.
        /// </summary>
        public const int CustomerLongLivedCacheProfile = 5;



    }
}

namespace Nop.Core.Caching
{
    /// <summary>
    /// Represents default values related to caching
    /// </summary>
    public static partial class NopCachingDefaults
    {
        /// <summary>
        /// Gets the default cache time in minutes
        /// </summary>
        public static int CacheTime => 60;

        /// <summary>
        /// Gets the key used to store the protection key list to Redis (used with the PersistDataProtectionKeysToRedis option enabled)
        /// </summary>
        public static string RedisDataProtectionKey => "Nop.DataProtectionKeys";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Entity type name
        /// </remarks>
        public static string NopEntityPrefixCacheKey => "Nop.{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Entity type name
        /// {1} : Entity id
        /// </remarks>
        public static string NopEntityCacheKey => "Nop.{0}.id-{1}";
    }
}
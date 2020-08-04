namespace Nop.Core.Caching
{
    /// <summary>
    /// Represents default values related to caching
    /// </summary>
    public static partial class NopCachingDefaults
    {
        /// <summary>
        /// Gets an algorithm used to create the hash value of identifiers need to cache
        /// </summary>
        public static string HashAlgorithm => "SHA1";

        /// <summary>
        /// Gets a key for caching entity by identifier
        /// </summary>
        /// <remarks>
        /// {0} : Entity type name
        /// {1} : Entity id
        /// </remarks>
        public static CacheKey EntityByIdCacheKey => new CacheKey("Nop.{0}.id-{1}");

        /// <summary>
        /// Gets a key for caching entities by identifiers
        /// </summary>
        /// <remarks>
        /// {0} : Entity type name
        /// {1} : Entity ids
        /// </remarks>
        public static CacheKey EntitiesByIdsCacheKey => new CacheKey("Nop.{0}.ids-{1}");

        /// <summary>
        /// Gets a key for caching all entities
        /// </summary>
        /// <remarks>
        /// {0} : Entity type name
        /// </remarks>
        public static CacheKey AllEntitiesCacheKey => new CacheKey("Nop.{0}.all");

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : Entity type name
        /// </remarks>
        public static string EntityPrefix => "Nop.{0}.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : Entity type name
        /// </remarks>
        public static string EntityByIdPrefix => "Nop.{0}.id-";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : Entity type name
        /// </remarks>
        public static string EntitiesByIdsPrefix => "Nop.{0}.ids-";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : Entity type name
        /// </remarks>
        public static string AllEntitiesPrefix => "Nop.{0}.all";
    }
}
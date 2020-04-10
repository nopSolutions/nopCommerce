using Nop.Core.Caching;

namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to common services
    /// </summary>
    public static partial class NopCommonCachingDefaults
    {
        #region Address attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey AddressAttributesAllCacheKey => new CacheKey("Nop.addressattribute.all");
        
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : address attribute ID
        /// </remarks>
        public static CacheKey AddressAttributeValuesAllCacheKey => new CacheKey("Nop.addressattributevalue.all-{0}");
       
        #endregion
        
        #region Generic attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : key group
        /// </remarks>
        public static CacheKey GenericAttributeCacheKey => new CacheKey("Nop.genericattribute.{0}-{1}");

        #endregion
    }
}
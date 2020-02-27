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
        public static string AddressAttributesAllCacheKey => "Nop.addressattribute.all";
        
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : address attribute ID
        /// </remarks>
        public static string AddressAttributeValuesAllCacheKey => "Nop.addressattributevalue.all-{0}";
       
        #endregion
        
        #region Generic attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : key group
        /// </remarks>
        public static string GenericAttributeCacheKey => "Nop.genericattribute.{0}-{1}";

        #endregion
    }
}
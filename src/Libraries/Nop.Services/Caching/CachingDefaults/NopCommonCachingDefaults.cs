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
        public static string AddressAttributesByIdCacheKey => "Nop.addressattribute.id-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : address attribute ID
        /// </remarks>
        public static string AddressAttributeValuesAllCacheKey => "Nop.addressattributevalue.all-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : address attribute value ID
        /// </remarks>
        public static string AddressAttributeValuesByIdCacheKey => "Nop.addressattributevalue.id-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string AddressAttributesPrefixCacheKey => "Nop.addressattribute.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string AddressAttributeValuesPrefixCacheKey => "Nop.addressattributevalue.";

        #endregion

        #region Addresses

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : address ID
        /// </remarks>
        public static string AddressesByIdCacheKey => "Nop.address.id-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string AddressesPrefixCacheKey => "Nop.address.";

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

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string GenericAttributePrefixCacheKey => "Nop.genericattribute.";

        #endregion
    }
}
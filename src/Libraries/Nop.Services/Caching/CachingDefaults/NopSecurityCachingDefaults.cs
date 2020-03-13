using Nop.Core.Caching;

namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to security services
    /// </summary>
    public static partial class NopSecurityCachingDefaults
    {
        #region Access control list

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static CacheKey AclRecordByEntityIdNameCacheKey => new CacheKey("Nop.aclrecord.entityid-name-{0}-{1}");

        #endregion

        #region Permissions

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : permission system name
        /// {1} : customer role ID
        /// </remarks>
        public static CacheKey PermissionsAllowedCacheKey => new CacheKey("Nop.permission.allowed-{0}-{1}", PermissionsAllowedPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : permission system name
        /// </remarks>
        public static string PermissionsAllowedPrefixCacheKey => "Nop.permission.allowed-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer role ID
        /// </remarks>
        public static CacheKey PermissionsAllByCustomerRoleIdCacheKey => new CacheKey("Nop.permission.allbycustomerroleid-{0}", PermissionsAllByCustomerRoleIdPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string PermissionsAllByCustomerRoleIdPrefixCacheKey => "Nop.permission.allbycustomerroleid";

        #endregion
    }
}
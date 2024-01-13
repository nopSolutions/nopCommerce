using Nop.Core.Caching;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security;

/// <summary>
/// Represents default values related to security services
/// </summary>
public static partial class NopSecurityDefaults
{
    #region reCAPTCHA

    /// <summary>
    /// Gets a reCAPTCHA script URL
    /// </summary>
    /// <remarks>
    /// {0} : Id of recaptcha instance on page
    /// {1} : Render type
    /// {2} : language if exists
    /// </remarks>
    public static string RecaptchaScriptPath => "api.js?onload=onloadCallback{0}&render={1}{2}";

    /// <summary>
    /// Gets a reCAPTCHA validation URL
    /// </summary>
    /// <remarks>
    /// {0} : private key
    /// {1} : response value
    /// {2} : IP address
    /// </remarks>
    public static string RecaptchaValidationPath => "api/siteverify?secret={0}&response={1}&remoteip={2}";

    #endregion

    #region Caching defaults

    #region Access control list

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : entity ID
    /// {1} : entity name
    /// </remarks>
    public static CacheKey AclRecordCacheKey => new("Nop.aclrecord.{0}-{1}");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : entity name
    /// </remarks>
    public static CacheKey EntityAclRecordExistsCacheKey => new("Nop.aclrecord.exists.{0}");

    #endregion

    #region Permissions

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : permission system name
    /// {1} : customer role ID
    /// </remarks>
    public static CacheKey PermissionAllowedCacheKey => new("Nop.permissionrecord.allowed.{0}-{1}", PermissionAllowedPrefix);

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    /// <remarks>
    /// {0} : permission system name
    /// </remarks>
    public static string PermissionAllowedPrefix => "Nop.permissionrecord.allowed.{0}";

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : customer role ID
    /// </remarks>
    public static CacheKey PermissionRecordsAllCacheKey => new("Nop.permissionrecord.all.{0}", NopEntityCacheDefaults<PermissionRecord>.AllPrefix);

    #endregion

    #endregion
}
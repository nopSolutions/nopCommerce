namespace Nop.Services.Security
{
    /// <summary>
    /// Represents default values related to security services
    /// </summary>
    public static partial class NopSecurityDefaults
    {
        #region Access control list

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static string AclRecordByEntityIdNameCacheKey => "Nop.aclrecord.entityid-name-{0}-{1}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string AclRecordPrefixCacheKey => "Nop.aclrecord.";

        #endregion

        #region Permissions

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer role ID
        /// {1} : permission system name
        /// </remarks>
        public static string PermissionsAllowedCacheKey => "Nop.permission.allowed-{0}-{1}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer role ID
        /// </remarks>
        public static string PermissionsAllByCustomerRoleIdCacheKey => "Nop.permission.allbycustomerroleid-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string PermissionsPrefixCacheKey => "Nop.permission.";

        #endregion

        #region reCAPTCHA

        /// <summary>
        /// Gets a base reCAPTCHA API URL
        /// </summary>
        public static string RecaptchaApiUrl => "https://www.google.com/recaptcha/";

        /// <summary>
        /// Gets a reCAPTCHA script URL
        /// </summary>
        /// <remarks>
        /// {0} : Id of recaptcha instance on page
        /// {1} : language if exists
        /// </remarks>
        public static string RecaptchaScriptPath => "api.js?onload=onloadCallback{0}&render=explicit{1}";

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
    }
}
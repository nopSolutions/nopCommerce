namespace Nop.Services.Defaults
{
    /// <summary>
    /// Represents default values related to security services
    /// </summary>
    public static partial class NopSecurityDefaults
    {
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
    }
}
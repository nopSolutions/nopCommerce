namespace Nop.Services.Authentication.MultiFactor
{
    public enum MultiFactorAuthenticationType
    {
        /// <summary>
        /// TOTP (Time-based One-time Password Algorithm) by "Microsoft Authenticator" App or "Google Authenticator" App etc.
        /// </summary>
        ApplicationVerification = 0,

        /// <summary>
        /// SMS verification
        /// </summary>
        SMSVerification = 1,

        /// <summary>
        /// Email verification
        /// </summary>
        EmailVerification = 2,
    }
}


namespace Nop.Plugin.ExternalAuth.Facebook
{
    /// <summary>
    /// Default values used by the Facebook authentication middleware
    /// </summary>
    public class FacebookAuthenticationDefaults
    {
        /// <summary>
        /// System name of the external authentication method
        /// </summary>
        public const string ProviderSystemName = "ExternalAuth.Facebook";

        /// <summary>
        /// Name of the view component to display plugin in public store
        /// </summary>
        public const string ViewComponentName = "FacebookAuthentication";
    }
}
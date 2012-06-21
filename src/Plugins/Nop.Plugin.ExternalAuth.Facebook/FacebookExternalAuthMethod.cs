using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.ExternalAuth.Facebook
{
    /// <summary>
    /// Facebook externalAuth processor
    /// </summary>
    public class FacebookExternalAuthMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public FacebookExternalAuthMethod(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ExternalAuthFacebook";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.ExternalAuth.Facebook.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for displaying plugin in public store
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPublicInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "ExternalAuthFacebook";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.ExternalAuth.Facebook.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new FacebookExternalAuthSettings()
            {
                ClientKeyIdentifier = "",
                ClientSecret = "",
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.Login", "Login using Facebook account");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier", "Client key identifier");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier.Hint", "Enter your client key identifier here.");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret", "Client secret");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret.Hint", "Enter your client secret here.");

            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<FacebookExternalAuthSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.Login");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier.Hint");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret.Hint");

            base.Uninstall();
        }

        #endregion
        
    }
}

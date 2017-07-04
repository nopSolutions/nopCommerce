using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.ExternalAuth.Facebook
{
    /// <summary>
    /// Represents method for the authentication with Facebook account
    /// </summary>
    public class FacebookAuthenticationMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public FacebookAuthenticationMethod(ISettingService settingService,
            IWebHelper webHelper)
        {
            this._settingService = settingService;
            this._webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/FacebookAuthentication/Configure";
        }

        /// <summary>
        /// Gets a view component for displaying plugin in public store
        /// </summary>
        /// <param name="viewComponentName">View component name</param>
        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "FacebookAuthentication";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new FacebookExternalAuthSettings());

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier", "App ID/API Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier.Hint", "Enter your app ID/API key here. You can find it on your FaceBook application page.");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret", "App Secret");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret.Hint", "Enter your app secret here. You can find it on your FaceBook application page.");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<FacebookExternalAuthSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier.Hint");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret.Hint");

            base.Uninstall();
        }

        #endregion
    }
}

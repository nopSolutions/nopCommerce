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

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public FacebookAuthenticationMethod(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper)
        {
            this._localizationService = localizationService;
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
        /// Gets a name of a view component for displaying plugin in public store
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return FacebookAuthenticationDefaults.ViewComponentName;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new FacebookExternalAuthSettings());

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier", "App ID/API Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier.Hint", "Enter your app ID/API key here. You can find it on your FaceBook application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret", "App Secret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret.Hint", "Enter your app secret here. You can find it on your FaceBook application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.Instructions", "<p>To configure authentication with Facebook, please follow these steps:<br/><br/><ol><li>Navigate to the <a href=\"https://developers.facebook.com/apps\" target =\"_blank\" > Facebook for Developers</a> page and sign in. If you don't already have a Facebook account, use the <b>Sign up for Facebook</b> link on the login page to create one.</li><li>Tap the <b>+ Add a New App</b> button in the upper right corner to create a new App ID. (If this is your first app with Facebook, the text of the button will be <b>Create a New App</b>.)</li><li>Fill out the form and tap the <b>Create App ID</b>  button.</li><li>The <b>Product Setup</b> page is displayed, letting you select the features for your new app. Select the option <b>Facebook Login</b> and press <b>Set up</b>.</li><li>Click on <b>Settings</b> on the left menu and in section <b>Client OAuth Settings</b> select the field <b>Valid OAuth Redirect URIs</b></li><li>Enter \"YourStoreUrl/signin-facebook\" in that field.</li><li>Click <b>Save Changes</b>.</li><li>Click the <b>Dashboard</b> link in the left navigation.</li><li>Copy your App ID and App secret below.</li></ol><br/><br/></p>");

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
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.Instructions");

            base.Uninstall();
        }

        #endregion
    }
}
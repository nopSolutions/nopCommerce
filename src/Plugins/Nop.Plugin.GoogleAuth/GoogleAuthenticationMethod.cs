using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.GoogleAuth.Components;
using Nop.Plugin.GoogleAuth;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Cms;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.GoogleAuth
{
    /// <summary>
    /// Represents method for the authentication with Facebook account
    /// </summary>
    public class GoogleAuthenticationMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public GoogleAuthenticationMethod(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
        }

        //bool IWidgetPlugin.HideInWidgetList => false;

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/GoogleAuthentication/Configure";
        }

        /// <summary>
        /// Gets a type of a view component for displaying plugin in public store
        /// </summary>
        /// <returns>View component type</returns>
        public Type GetPublicViewComponent()
        {
            return typeof(GoogleAuthenticationViewComponent);
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new GoogleExternalAuthSettings());

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.GoogleAuth.AuthenticationDataDeletedSuccessfully"] = "Data deletion request completed",
                ["Plugins.GoogleAuth.AuthenticationDataExist"] = "Data deletion request is pending, please contact the admin",
                ["Plugins.GoogleAuth.ClientKeyIdentifier"] = "Client ID",
                ["Plugins.GoogleAuth.ClientKeyIdentifier.Hint"] = "Enter your app ID/API key here. You can find it on your Google application page.",
                ["Plugins.GoogleAuth.ClientSecret"] = "Client Secret",
                ["Plugins.GoogleAuth.ClientSecret.Hint"] = "Enter your app secret here. You can find it on your Google application page.",
                ["Plugins.GoogleAuth.Instructions"] = "<p>To configure authentication with Google, please follow these steps:<br/><br/><ol><li>Navigate to the for Google API & Services and select create project.</li><li>Fill out the form and tap the Create button.</li><li>In the Oauth consent screen of the Dashboard.</li><li>Select User Type External and CREATE.</li><li>In the App information dialog Provide an app name for the app user support email and developer contact information.</li><li>Step through the Scopes and the test users step.</li><li>Review the OAuth consent screen and go back to the app Dashboard.</li><li>In the Credentials tab of the application Dashboard select CREATE CREDENTIALS OAuth client ID.</li><li>Select Application type Web application and choose a name.</li><li>In the Authorized redirect URIs section, select ADD URI to set the redirect URI. Example redirect URI: \"{0:s}signin-google\" into the <b>Valid OAuth Redirect URIs</b> field.</li><li>Copy your Client ID and Client secret below.</li></ol><br/><br/></p>"
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<GoogleExternalAuthSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.GoogleAuth");

            await base.UninstallAsync();
        }

        //Type IWidgetPlugin.GetWidgetViewComponent(string widgetZone)
        //{
        //    return typeof(GoogleAuthenticationViewComponent);
        //}

        //Task<IList<string>> IWidgetPlugin.GetWidgetZonesAsync()
        //{
        //    return Task.FromResult<IList<string>>(new List<string> {
        //        PublicWidgetZones.LoginBottom
        //    });
        //}

        #endregion
    }
}
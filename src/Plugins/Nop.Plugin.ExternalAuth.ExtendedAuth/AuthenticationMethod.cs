using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Plugin.ExternalAuth.ExtendedAuth.Infrastructure;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using System.Threading.Tasks;
using Nop.Web.Framework.Menu;
using System;
using System.Linq;

namespace Nop.Plugin.ExternalAuth.ExtendedAuthentication
{
    public class AuthenticationMethod : BasePlugin, IExternalAuthenticationMethod, IAdminMenuPlugin
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IPluginService _pluginService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ILocalizationService _localizationService;
        private readonly WidgetSettings _widgetSettings;
        private readonly Services.Tasks.IScheduleTaskService _scheduleTaskService;

        #endregion

        #region Ctor

        public AuthenticationMethod(
            IWebHelper webHelper,
            IPluginService pluginService,
            IEmailAccountService emailAccountService,
            IQueuedEmailService queuedEmailService,
            ILocalizationService localizationService,
            ISettingService settingService,
            WidgetSettings widgetSettings,
            Services.Tasks.IScheduleTaskService scheduleTaskService)
        {
            _webHelper = webHelper;
            _pluginService = pluginService;
            _emailAccountService = emailAccountService;
            _queuedEmailService = queuedEmailService;
            _localizationService = localizationService;
            _settingService = settingService;
            _widgetSettings = widgetSettings;
            _scheduleTaskService = scheduleTaskService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Authentication/Configure";
        }

        public SiteMapNode BuildMenuItem()
        {
            var menuItemBuilder = new SiteMapNode()
            {
                Title = "External authentication",   // Title for your Custom Menu Item
                SystemName = "ExternalAuth",
                IconClass = "fa fa-dot-circle-o",
                Url = "", // Path of the action link
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
            };

            var subMenuItem = new SiteMapNode()
            {
                Title = "Configure",
                Url = _webHelper.GetStoreLocation() + "Admin/Authentication/Configure",
                Visible = true,
                IconClass = "fa fa-genderless",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } },
            };
            menuItemBuilder.ChildNodes.Add(subMenuItem);
            return menuItemBuilder;
        }

        public Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var menuItem = BuildMenuItem();
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName: "ExternalAuth.Facebook", LoadPluginsMode.NotInstalledOnly);
            if (pluginDescriptor != null && pluginDescriptor.Installed) /* is not enabled */
                throw new NopException("Please Uninstall Facebook authentication plugin to use this plugin");

            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.plugin.IsEnabled", "Enabled");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.plugin.IsEnabled.Hint", "Check if you want to Enable this plugin.");

            //locales for facebook
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier", "App ID/API Key");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier.Hint", "Enter your app ID/API key here. You can find it on your FaceBook application page.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Facebook.ClientSecret", "App Secret");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Facebook.ClientSecret.Hint", "Enter your app secret here. You can find it on your FaceBook application page.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Facebook.IsEnabled", "Is enabled");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Facebook.IsEnabled.Hint", "Indicates whether the facebook login is enabled/active.");

            //locales for Twitter
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Twitter.ConsumerKey", "Consumer ID/API Key");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Twitter.ConsumerKey.Hint", "Enter your Consumer ID/API key here. You can find it on your Twitter application page.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Twitter.ConsumerSecret", "Consumer Secret");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Twitter.ConsumerSecret.Hint", "Enter your Consumer secret here. You can find it on your Twitter application page.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Twitter.IsEnabled", "Is enabled");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Twitter.IsEnabled.Hint", "Indicates whether the twitter login is enabled/active.");

            //locales for Google
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Google.ClientId", "Client ID/API Key");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Google.ClientId.Hint", "Enter your Client ID/API key here. You can find it on your Google application page.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Google.ClientSecret", "Client Secret");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Google.ClientSecret.Hint", "Enter your Client secret here. You can find it on your Google application page.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Google.IsEnabled", "Is enabled");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Google.IsEnabled.Hint", "Indicates whether the google login is enabled/active.");

            //locales for Microsoft
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Microsoft.ClientId", "Client ID/API Key");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Microsoft.ClientId.Hint", "Enter your Client ID/API key here. You can find it on your Microsoft application page.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Microsoft.ClientSecret", "Client Secret");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Microsoft.ClientSecret.Hint", "Enter your Client secret here. You can find it on your Microsoft application page.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Microsoft.IsEnabled", "Is enabled");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.Microsoft.IsEnabled.Hint", "Indicates whether the microsoft login is enabled/active.");

            //locales for LinkedIn
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.LinkedIn.ClientId", "Client ID/API Key");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.LinkedIn.ClientId.Hint", "Enter your Client ID/API key here. You can find it on your LinkedIn application page.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.LinkedIn.ClientSecret", "Client Secret");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.LinkedIn.ClientSecret.Hint", "Enter your Client secret here. You can find it on your LinkedIn application page.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.LinkedIn.IsEnabled", "Is enabled");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExternalAuth.LinkedIn.IsEnabled.Hint", "Indicates whether the linkedIn login is enabled/active.");

            _widgetSettings.ActiveWidgetSystemNames.Add(AuthenticationDefaults.PluginSystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);

            if (await _scheduleTaskService.GetTaskByTypeAsync(WeekdayProductRotation.PRODUCT_ROTATION_TASK) == null)
            {
                await _scheduleTaskService.InsertTaskAsync(new Core.Domain.Tasks.ScheduleTask
                {
                    Enabled = false,
                    Name = "Product rotation task",
                    Type = WeekdayProductRotation.PRODUCT_ROTATION_TASK,
                    Seconds = 5 * 60
                });
            }
            
            if (await _scheduleTaskService.GetTaskByTypeAsync(CompanyAddressPropogator.CompanyAddressPropogatorTask) == null)
            {
                await _scheduleTaskService.InsertTaskAsync(new Core.Domain.Tasks.ScheduleTask
                {
                    Enabled = false,
                    Name = CompanyAddressPropogator.CompanyAddressPropogatorTaskName,
                    Type = CompanyAddressPropogator.CompanyAddressPropogatorTask,
                    Seconds = 60 * 60
                });
            }

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.plugin.IsEnabled");

            //locales
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Facebook.ClientSecret");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Facebook.ClientSecret.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Facebook.IsEnabled");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Facebook.IsEnabled.Hint");

            //locales for Twitter
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Twitter.ConsumerKey");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Twitter.ConsumerKey.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Twitter.ConsumerSecret");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Twitter.ConsumerSecret.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Twitter.IsEnabled");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Twitter.IsEnabled.Hint");

            //locales for Google
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Google.ClientId");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Google.ClientId.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Google.ClientSecret");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Google.ClientSecret.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Google.IsEnabled");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Google.IsEnabled.Hint");

            //locales for Microsoft
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Microsoft.ClientId");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Microsoft.ClientId.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Microsoft.ClientSecret");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Microsoft.ClientSecret.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Microsoft.IsEnabled");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.Microsoft.IsEnabled.Hint");

            //locales for LinkedIn
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.LinkedIn.ClientId");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.LinkedIn.ClientId.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.LinkedIn.ClientSecret");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.LinkedIn.ClientSecret.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.LinkedIn.IsEnabled");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExternalAuth.LinkedIn.IsEnabled.Hint");

            await base.UninstallAsync();
        }

        public string GetPublicViewComponentName()
        {
            return "Authentication";
        }

        #endregion
    }
}

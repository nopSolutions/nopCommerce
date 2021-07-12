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
using Nop.Services.Tasks;
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
        private readonly IScheduleTaskService _scheduleTaskService;

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
            IScheduleTaskService scheduleTaskService)
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

            var SubMenuItem = new SiteMapNode()
            {
                Title = "Configure",
                Url = _webHelper.GetStoreLocation() + "Admin/Authentication/Configure",
                Visible = true,
                IconClass = "fa fa-genderless",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } },
            };
            menuItemBuilder.ChildNodes.Add(SubMenuItem);
            return menuItemBuilder;
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = BuildMenuItem();
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            var pluginDescriptor = _pluginService.GetPluginDescriptorBySystemName<IPlugin>(systemName: "ExternalAuth.Facebook", LoadPluginsMode.NotInstalledOnly);
            if (pluginDescriptor != null && pluginDescriptor.Installed) /* is not enabled */
                throw new NopException("Please Uninstall Facebook authentication plugin to use this plugin");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.plugin.IsEnabled", "Enabled");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.plugin.IsEnabled.Hint", "Check if you want to Enable this plugin.");

            //locales for facebook
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier", "App ID/API Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier.Hint", "Enter your app ID/API key here. You can find it on your FaceBook application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret", "App Secret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret.Hint", "Enter your app secret here. You can find it on your FaceBook application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.IsEnabled", "Is enabled");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Facebook.IsEnabled.Hint", "Indicates whether the facebook login is enabled/active.");

            //locales for Twitter
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Twitter.ConsumerKey", "Consumer ID/API Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Twitter.ConsumerKey.Hint", "Enter your Consumer ID/API key here. You can find it on your Twitter application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Twitter.ConsumerSecret", "Consumer Secret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Twitter.ConsumerSecret.Hint", "Enter your Consumer secret here. You can find it on your Twitter application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Twitter.IsEnabled", "Is enabled");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Twitter.IsEnabled.Hint", "Indicates whether the twitter login is enabled/active.");

            //locales for Google
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Google.ClientId", "Client ID/API Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Google.ClientId.Hint", "Enter your Client ID/API key here. You can find it on your Google application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Google.ClientSecret", "Client Secret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Google.ClientSecret.Hint", "Enter your Client secret here. You can find it on your Google application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Google.IsEnabled", "Is enabled");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Google.IsEnabled.Hint", "Indicates whether the google login is enabled/active.");

            //locales for Microsoft
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Microsoft.ClientId", "Client ID/API Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Microsoft.ClientId.Hint", "Enter your Client ID/API key here. You can find it on your Microsoft application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Microsoft.ClientSecret", "Client Secret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Microsoft.ClientSecret.Hint", "Enter your Client secret here. You can find it on your Microsoft application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Microsoft.IsEnabled", "Is enabled");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Microsoft.IsEnabled.Hint", "Indicates whether the microsoft login is enabled/active.");

            //locales for LinkedIn
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.LinkedIn.ClientId", "Client ID/API Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.LinkedIn.ClientId.Hint", "Enter your Client ID/API key here. You can find it on your LinkedIn application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.LinkedIn.ClientSecret", "Client Secret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.LinkedIn.ClientSecret.Hint", "Enter your Client secret here. You can find it on your LinkedIn application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.LinkedIn.IsEnabled", "Is enabled");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.LinkedIn.IsEnabled.Hint", "Indicates whether the linkedIn login is enabled/active.");

            #region Notification

            var emailAccount = _emailAccountService.GetAllEmailAccounts().Where(x => x.UseDefaultCredentials == true).FirstOrDefault();
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
            if (emailAccount != null)
            {
                var currentstore = EngineContext.Current.Resolve<IStoreContext>().CurrentStore;
                string body = "Our store <a href='" + currentstore.Url + "'>" + currentstore.Name + "</a> staring to use External authentication  plugin 4.2";
                var email = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.High,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = "SangeetShah143@Gmail.com",
                    ToName = "Sangeet Shah",
                    Subject = "Welcome",
                    Body = body,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                };
                _queuedEmailService.InsertQueuedEmail(email);
            }
            #endregion Notification

            _widgetSettings.ActiveWidgetSystemNames.Add(AuthenticationDefaults.PluginSystemName);
            _settingService.SaveSetting(_widgetSettings);

            if (_scheduleTaskService.GetTaskByType(WeekdayProductRotation.PRODUCT_ROTATION_TASK) == null)
            {
                _scheduleTaskService.InsertTask(new Core.Domain.Tasks.ScheduleTask
                {
                    Enabled = true,
                    Name = "Product rotation task",
                    Type = WeekdayProductRotation.PRODUCT_ROTATION_TASK,
                    Seconds = 5 * 60
                });
            }

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.plugin.IsEnabled");

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.ClientSecret.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.IsEnabled");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Facebook.IsEnabled.Hint");

            //locales for Twitter
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Twitter.ConsumerKey");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Twitter.ConsumerKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Twitter.ConsumerSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Twitter.ConsumerSecret.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Twitter.IsEnabled");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Twitter.IsEnabled.Hint");

            //locales for Google
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Google.ClientId");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Google.ClientId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Google.ClientSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Google.ClientSecret.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Google.IsEnabled");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Google.IsEnabled.Hint");

            //locales for Microsoft
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Microsoft.ClientId");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Microsoft.ClientId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Microsoft.ClientSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Microsoft.ClientSecret.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Microsoft.IsEnabled");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Microsoft.IsEnabled.Hint");

            //locales for LinkedIn
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.LinkedIn.ClientId");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.LinkedIn.ClientId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.LinkedIn.ClientSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.LinkedIn.ClientSecret.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.LinkedIn.IsEnabled");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.LinkedIn.IsEnabled.Hint");

            #region Notification

            var emailAccount = _emailAccountService.GetAllEmailAccounts().Where(x => x.UseDefaultCredentials == true).FirstOrDefault();
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
            if (emailAccount != null)
            {
                var currentstore = EngineContext.Current.Resolve<IStoreContext>().CurrentStore;
                string body = "Our store <a href='" + currentstore.Url + "'>" + currentstore.Name + "</a> not happy with External authentication  plugin";
                var email = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.High,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = "SangeetShah143@Gmail.com",
                    ToName = "Sangeet Shah",
                    Subject = "Welcome",
                    Body = body,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                };
                _queuedEmailService.InsertQueuedEmail(email);
            }
            #endregion Notification

            base.Uninstall();
        }

        public string GetPublicViewComponentName()
        {
            return "Authentication";
        }

        #endregion
    }
}

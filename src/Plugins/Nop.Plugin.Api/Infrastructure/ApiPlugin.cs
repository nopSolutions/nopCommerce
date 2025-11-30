using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Api.Domain;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Api.Infrastructure
{
    public class ApiPlugin : BasePlugin
    {
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        public ApiPlugin(
            ISettingService settingService,
            IWorkContext workContext,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IWebHelper webHelper)
        {
            _settingService = settingService;
            _workContext = workContext;
            _customerService = customerService;
            _localizationService = localizationService;
            _webHelper = webHelper;
        }

        public override async Task InstallAsync()
        {
            //locales

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                { "Plugins.Api", "Api plugin" },
                { "Plugins.Api.Admin.Menu.ManageClients", "Manage Api Clients" },
                { "Plugins.Api.Admin.Configure", "Configure Web Api" },
                { "Plugins.Api.Admin.GeneralSettings", "General Settings" },
                { "Plugins.Api.Admin.EnableApi", "Enable Api" },
                { "Plugins.Api.Admin.EnableApi.Hint", "By checking this settings you can Enable/Disable the Web Api" },
                { "Plugins.Api.Admin.TokenExpiryInDays", "Access token expiration in days" },
                { "Plugins.Api.Admin.Menu.Title", "API" },
                { "Plugins.Api.Admin.Menu.Settings.Title", "Settings" },
                { "Plugins.Api.Admin.Page.Settings.Title", "Api Settings" },
                { "Plugins.Api.Admin.Settings.GeneralSettingsTitle", "General Settings" },
                { "Plugins.Api.Admin.Edit", "Edit" },
                { "Api.Categories.Fields.Id.Invalid", "Id is invalid" },
                { "Api.InvalidPropertyType", "Invalid Property Type" },
                { "Api.InvalidType", "Invalid {0} type" },
                { "Api.InvalidRequest", "Invalid request" },
                { "Api.InvalidRootProperty", "Invalid root property" },
                { "Api.NoJsonProvided", "No Json provided" },
                { "Api.InvalidJsonFormat", "Json format is invalid" },
                { "Api.Category.InvalidImageAttachmentFormat", "Invalid image attachment base64 format" },
                { "Api.Category.InvalidImageSrc", "Invalid image source" },
                { "Api.Category.InvalidImageSrcType", "You have provided an invalid image source/attachment " },
            });



            await _settingService.SaveSettingAsync(new ApiSettings());

            var apiRole = await _customerService.GetCustomerRoleBySystemNameAsync(Constants.Roles.ApiRoleSystemName);

            if (apiRole == null)
            {
                apiRole = new CustomerRole
                {
                    Name = Constants.Roles.ApiRoleName,
                    Active = true,
                    SystemName = Constants.Roles.ApiRoleSystemName
                };

                await _customerService.InsertCustomerRoleAsync(apiRole);
            }
            else if (apiRole.Active == false)
            {
                apiRole.Active = true;
                await _customerService.UpdateCustomerRoleAsync(apiRole);
            }
            
            var activityLogTypeRepository = EngineContext.Current.Resolve<IRepository<ActivityLogType>>();
            var activityLogType = (await activityLogTypeRepository.GetAllAsync(query =>
            {
                return query.Where(x => x.SystemKeyword == "Api.TokenRequest");
            })).FirstOrDefault();

            if (activityLogType == null)
            {
                await activityLogTypeRepository.InsertAsync(new ActivityLogType
                {
                    SystemKeyword = "Api.TokenRequest",
                    Name = "API token request",
                    Enabled = true
                });
            }

            await base.InstallAsync();

            // Changes to Web.Config trigger application restart.
            // This doesn't appear to affect the Install function, but just to be safe we will made web.config changes after the plugin was installed.
            //_webConfigMangerHelper.AddConfiguration();
        }

        public override async Task UninstallAsync()
        {
            //locales
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Api");

            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Title");
            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Settings.Title");

            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Configure");
            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.GeneralSettings");
            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.EnableApi");
            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.EnableApi.Hint");

            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Settings.GeneralSettingsTitle");
            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Edit");


            var apiRole = await _customerService.GetCustomerRoleBySystemNameAsync(Constants.Roles.ApiRoleSystemName);
            if (apiRole != null)
            {
                apiRole.Active = false;
                await _customerService.UpdateCustomerRoleAsync(apiRole);
            }
            
            var activityLogTypeRepository = EngineContext.Current.Resolve<IRepository<ActivityLogType>>();
            var activityLogType = (await activityLogTypeRepository.GetAllAsync(query =>
            {
                return query.Where(x => x.SystemKeyword.Equals("Api.TokenRequest"));
            })).FirstOrDefault();
            if (activityLogType != null)
            {
                activityLogType.Enabled = false;
                await activityLogTypeRepository.UpdateAsync(activityLogType);
            }

            await base.UninstallAsync();
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ApiAdmin/Settings";
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var workingLanguage = await _workContext.GetWorkingLanguageAsync();

            var pluginMenuName = await _localizationService.GetResourceAsync("Plugins.Api.Admin.Menu.Title", workingLanguage.Id, defaultValue: "API");

            var settingsMenuName = await _localizationService.GetResourceAsync("Plugins.Api.Admin.Menu.Settings.Title", workingLanguage.Id, defaultValue: "API");

            const string adminUrlPart = "Admin/";

            var pluginMainMenu = new SiteMapNode
            {
                Title = pluginMenuName,
                Visible = true,
                SystemName = "Api-Main-Menu",
                IconClass = "fa-genderless"
            };

            pluginMainMenu.ChildNodes.Add(new SiteMapNode
            {
                Title = settingsMenuName,
                Url = _webHelper.GetStoreLocation() + adminUrlPart + "ApiAdmin/Settings",
                Visible = true,
                SystemName = "Api-Settings-Menu",
                IconClass = "fa-genderless"
            });


            rootNode.ChildNodes.Add(pluginMainMenu);
        }
    }
}

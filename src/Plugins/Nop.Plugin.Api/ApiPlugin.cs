namespace Nop.Plugin.Api
{
    using IdentityServer4.EntityFramework.DbContexts;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;
    using Nop.Core;
    using Nop.Core.Infrastructure;
    using Nop.Plugin.Api.Data;
    using Nop.Plugin.Api.Domain;
    using Nop.Plugin.Api.Helpers;
    using Nop.Services.Configuration;
    using Nop.Services.Localization;
    using Nop.Services.Plugins;
    using Nop.Web.Framework.Menu;

    public class ApiPlugin : BasePlugin, IAdminMenuPlugin
    {
        //private readonly IWebConfigMangerHelper _webConfigMangerHelper;
        private readonly ApiObjectContext _objectContext;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        public ApiPlugin(ApiObjectContext objectContext,/*IWebConfigMangerHelper webConfigMangerHelper,*/ ISettingService settingService, IWorkContext workContext,
            ILocalizationService localizationService, IWebHelper webHelper
/*, IConfiguration configuration*/)
        {
            _objectContext = objectContext;
            //_webConfigMangerHelper = webConfigMangerHelper;
            _settingService = settingService;
            _workContext = workContext;
            _localizationService = localizationService;
            _webHelper = webHelper;
            //_configuration = configuration;
        }

        //private readonly IConfiguration _configuration;

        public override void Install()
        {
            var configManagerHelper = new NopConfigManagerHelper();

            // some of third party libaries that we use for WebHooks and Swagger use older versions
            // of certain assemblies so we need to redirect them to the those that nopCommerce uses
            // TODO: Upgrade 4.1. check this!
            //configManagerHelper.AddBindingRedirects();

            // required by the WebHooks support
            // TODO: Upgrade 4.1. check this!
            //configManagerHelper.AddConnectionString();

            _objectContext.Install();

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api", "Api plugin");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.ManageClients", "Manage Api Clients");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Configure", "Configure Web Api");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.GeneralSettings", "General Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi", "Enable Api");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi.Hint", "By checking this settings you can Enable/Disable the Web Api");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger", "Allow Requests From Swagger");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger.Hint", "Swagger is the documentation generation tool used for the API (/Swagger). It has a client that enables it to make GET requests to the API endpoints. By enabling this option you will allow all requests from the swagger client. Do Not Enable on live site, it is only for demo sites or local testing!!!");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.Title","API");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.Settings.Title","Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.Clients.Title", "Clients");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.Docs.Title", "Docs");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Settings.Title", "Api Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Clients.Title", "Api Clients");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Clients.Create.Title", "Add a new Api client");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Clients.Edit.Title", "Edit Api client");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Name", "Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Name.Hint", "Name Hint");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientId", "Client Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientId.Hint", "The id of the client");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientSecret", "Client Secret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientSecret.Hint", "The client secret is used during the authentication for obtaining the Access Token");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.CallbackUrl", "Callback Url");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.CallbackUrl.Hint", "The url where the Authorization code will be send");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.IsActive", "Is Active");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.IsActive.Hint", "You can use it to enable/disable the access to your store for the client");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.AddNew", "Add New Client");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Edit", "Edit");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Created", "Created");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Deleted", "Deleted");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.Name", "Name is required");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientId", "Client Id is required");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientSecret", "Client Secret is required");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.CallbackUrl", "Callback Url is required");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Settings.GeneralSettingsTitle", "General Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Edit", "Edit");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.BackToList", "Back To List");

            _localizationService.AddOrUpdatePluginLocaleResource("Api.Categories.Fields.Id.Invalid", "Id is invalid");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.InvalidPropertyType", "Invalid Property Type");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.InvalidType", "Invalid {0} type");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.InvalidRequest", "Invalid request");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.InvalidRootProperty", "Invalid root property");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.NoJsonProvided", "No Json provided");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.InvalidJsonFormat", "Json format is invalid");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.Category.InvalidImageAttachmentFormat", "Invalid image attachment base64 format");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.Category.InvalidImageSrc", "Invalid image source");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.Category.InvalidImageSrcType", "You have provided an invalid image source/attachment ");

            _localizationService.AddOrUpdatePluginLocaleResource("Api.WebHooks.CouldNotRegisterWebhook", "Could not register WebHook due to error: {0}");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.WebHooks.CouldNotRegisterDuplicateWebhook", "Could not register WebHook because a webhook with the same URI and Filters is already registered.");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.WebHooks.CouldNotUpdateWebhook", "Could not update WebHook due to error: {0}");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.WebHooks.CouldNotDeleteWebhook", "Could not delete WebHook due to error: {0}");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.WebHooks.CouldNotDeleteWebhooks", "Could not delete WebHooks due to error: {0}");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.WebHooks.InvalidFilters", "The following filters are not valid: '{0}'. A list of valid filters can be obtained from the path '{1}'.");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableLogging", "Enable Logging");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableLogging.Hint", "By enable logging you will see webhook messages in the Log. These messages are needed ONLY for diagnostic purposes. NOTE: A restart is required when changing this setting in order to take effect");

            ApiSettings settings = new ApiSettings
            {
                EnableApi = true,
                AllowRequestsFromSwagger = false
            };

            _settingService.SaveSetting(settings);           

            base.Install();

            // Changes to Web.Config trigger application restart.
            // This doesn't appear to affect the Install function, but just to be safe we will made web.config changes after the plugin was installed.
            //_webConfigMangerHelper.AddConfiguration();
        }

        public override void Uninstall()
        {
            _objectContext.Uninstall();

            var persistedGrantMigrator = EngineContext.Current.Resolve<PersistedGrantDbContext>().GetService<IMigrator>();
            persistedGrantMigrator.Migrate("0");
            
            var configurationMigrator = EngineContext.Current.Resolve<ConfigurationDbContext>().GetService<IMigrator>();
            configurationMigrator.Migrate("0");

            // TODO: Delete all resources
            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Api");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.ManageClients");

            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Title");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Settings.Title");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Clients.Title");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Docs.Title");

            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Configure");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.GeneralSettings");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.EnableApi");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.EnableApi.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger.Hint");

            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Name");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.ClientId");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.ClientSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.CallbackUrl");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.IsActive");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Edit");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Created");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Deleted");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.Name");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientId");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.CallbackUrl");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Settings.GeneralSettingsTitle");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Edit");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.BackToList");

            _localizationService.DeletePluginLocaleResource("Api.WebHooks.CouldNotRegisterWebhook");
            _localizationService.DeletePluginLocaleResource("Api.WebHooks.CouldNotRegisterDuplicateWebhook");
            _localizationService.DeletePluginLocaleResource("Api.WebHooks.CouldNotUpdateWebhook");
            _localizationService.DeletePluginLocaleResource("Api.WebHooks.CouldNotDeleteWebhook");
            _localizationService.DeletePluginLocaleResource("Api.WebHooks.CouldNotDeleteWebhooks");
            _localizationService.DeletePluginLocaleResource("Api.WebHooks.InvalidFilters");

            base.Uninstall();

            // Changes to Web.Config trigger application restart.
            // This doesn't appear to affect the uninstall function, but just to be safe we will made web.config changes after the plugin was uninstalled.
            //_webConfigMangerHelper.RemoveConfiguration();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            string pluginMenuName = _localizationService.GetResource("Plugins.Api.Admin.Menu.Title",languageId: _workContext.WorkingLanguage.Id, defaultValue: "API");

            string settingsMenuName = _localizationService.GetResource("Plugins.Api.Admin.Menu.Settings.Title", languageId: _workContext.WorkingLanguage.Id, defaultValue: "API");

            string manageClientsMenuName = _localizationService.GetResource("Plugins.Api.Admin.Menu.Clients.Title", languageId: _workContext.WorkingLanguage.Id, defaultValue: "API");

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

            pluginMainMenu.ChildNodes.Add(new SiteMapNode
            {
                Title = manageClientsMenuName,
                Url = _webHelper.GetStoreLocation() + adminUrlPart + "ManageClientsAdmin/List",
                Visible = true,
                SystemName = "Api-Clients-Menu",
                IconClass = "fa-genderless"
            });
            

            string pluginDocumentationUrl = "https://github.com/SevenSpikes/api-plugin-for-nopcommerce";
            
            pluginMainMenu.ChildNodes.Add(new SiteMapNode
                {
                    Title = _localizationService.GetResource("Plugins.Api.Admin.Menu.Docs.Title"),
                    Url = pluginDocumentationUrl,
                    Visible = true,
                    SystemName = "Api-Docs-Menu",
                    IconClass = "fa-genderless"
                });//TODO: target="_blank"
            

            rootNode.ChildNodes.Add(pluginMainMenu);
        }
    }
}

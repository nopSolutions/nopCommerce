using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using System.Threading.Tasks;

namespace Nop.Plugin.Api.Rest
{
    public class Plugin : BasePlugin, IMiscPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INopUrlHelper _nopUrlHelper;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public Plugin(ILocalizationService localizationService,
            INopUrlHelper nopUrlHelper,
            ISettingService settingService)
        {
            _localizationService = localizationService;
            _nopUrlHelper = nopUrlHelper;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _nopUrlHelper.RouteUrl(ApiRestDefaults.ConfigurationRouteName);
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugin.Api.Rest.Title"] = "REST API",
                ["Plugin.Api.Rest.Description"] = "The API is published as two documents, one for the back office and one for the public store, mirroring the official nopCommerce Web API. Use the Swagger UI to switch between them from the definition dropdown. Both links are served by the plugin itself and require no host configuration.",
                ["Plugin.Api.Rest.SwaggerUi"] = "Swagger UI",
                ["Plugin.Api.Rest.SwaggerBackendJson"] = "Backend JSON (back office)",
                ["Plugin.Api.Rest.SwaggerFrontendJson"] = "Public store JSON",
                ["Plugin.Api.Rest.OpenSwaggerUi"] = "Open Swagger UI",
                ["Plugin.Api.Rest.OpenSwaggerBackendJson"] = "Open backend JSON",
                ["Plugin.Api.Rest.OpenSwaggerFrontendJson"] = "Open public store JSON",
                ["Plugin.Api.Rest.Authentication"] = "Authentication",
                ["Plugin.Api.Rest.Authentication.Description"] = "Call the token endpoint with your credentials to get a bearer token, then send it as 'Authorization: Bearer <token>'. For server to server calls you can send the shared API key below in the X-Api-Key header instead. Leaving the key field empty keeps the token endpoints disabled.",
                ["Plugin.Api.Rest.GenerateApiKey"] = "Generate API key",
                ["Plugin.Api.Rest.ApiKeyGenerated"] = "A new API key was generated and saved.",
                ["Plugin.Api.Rest.ApiKey"] = "API key",
                ["Plugin.Api.Rest.ApiKey.Placeholder"] = "Click 'Generate API key' to create one",
                ["Plugin.Api.Rest.ApiKey.Help"] = "Leave the field empty to keep the current key. Treat it like a password: anyone holding it can modify the catalog, orders and customers. It also signs the bearer tokens, so changing it invalidates every token already issued.",
                ["Plugin.Api.Rest.RateLimitPerMinute"] = "Requests per minute",
                ["Plugin.Api.Rest.RateLimitPerMinute.Help"] = "Maximum number of API requests a single client may send per minute. Set to 0 to disable the limit.",
                ["Plugin.Api.Rest.RequireApiKeyForReads"] = "Require a credential for reads",
                ["Plugin.Api.Rest.RequireApiKeyForReads.Help"] = "When enabled, anonymous callers get 401 on the back office reads unless they send the API key or a bearer token. Leave it off to browse the catalog anonymously, but note the back office customer and order reads are then public too. A customer's own data under /api/rest/customer/me is never public, and the storefront catalog needs a customer token instead of the API key, which is refused there.",
                ["Plugin.Api.Rest.AdminTokenLifetimeHours"] = "Admin token lifetime (hours)",
                ["Plugin.Api.Rest.AdminTokenLifetimeHours.Help"] = "How long a token issued to an administrator stays valid. It is shorter than the customer token because it grants write access to the catalog, orders and customers. Only affects tokens issued after saving.",
                ["Plugin.Api.Rest.CustomerTokenLifetimeDays"] = "Customer token lifetime (days)",
                ["Plugin.Api.Rest.CustomerTokenLifetimeDays.Help"] = "How long a token issued to a store customer stays valid. Such a token is scoped to that customer's own data. Only affects tokens issued after saving."
            });

            // write every default into the database on install. Without this the setting rows do not exist at
            //all, and a missing row silently resolves to the C# default instead of the value the admin
            //set. That is how a "Require a credential for reads" that had been ticked and saved could
            //still leave the customer and order reads anonymous: the save had been discarded, nothing was
            //written, and nothing reported that the value being used was an unstored default.
            await _settingService.SaveSettingAsync(new ApiRestSettings
            {
                ApiKey = string.Empty,
                RateLimitPerMinute = 60,
                RequireApiKeyForReads = false,
                AdminTokenLifetimeHours = 24,
                CustomerTokenLifetimeDays = 7
            }, storeId: 0);

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourcesAsync("Plugin.Api.Rest");
            await _settingService.DeleteSettingAsync<ApiRestSettings>();

            // cleanup plugin data if necessary
            await base.UninstallAsync();
        }

        #endregion
    }
}

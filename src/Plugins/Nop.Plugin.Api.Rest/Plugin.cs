using Nop.Services.Plugins;
using Nop.Services.Common;
using Nop.Services.Localization;
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

        #endregion

        #region Ctor

        public Plugin(ILocalizationService localizationService,
            INopUrlHelper nopUrlHelper)
        {
            _localizationService = localizationService;
            _nopUrlHelper = nopUrlHelper;
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
                ["Plugin.Api.Rest.Description"] = "Use the links below to explore the endpoints exposed by this plugin. Both links are served by the plugin itself and require no host configuration.",
                ["Plugin.Api.Rest.SwaggerUi"] = "Swagger UI",
                ["Plugin.Api.Rest.SwaggerJson"] = "Swagger JSON",
                ["Plugin.Api.Rest.OpenSwaggerUi"] = "Open Swagger UI",
                ["Plugin.Api.Rest.OpenSwaggerJson"] = "Open Swagger JSON",
                ["Plugin.Api.Rest.Authentication"] = "Authentication",
                ["Plugin.Api.Rest.Authentication.Description"] = "The write operations require the API key below. Send it either in the X-Api-Key header or as an Authorization: Bearer token. Leaving the field empty keeps the write operations disabled.",
                ["Plugin.Api.Rest.GenerateApiKey"] = "Generate API key",
                ["Plugin.Api.Rest.ApiKeyGenerated"] = "A new API key was generated and saved.",
                ["Plugin.Api.Rest.ApiKey"] = "API key",
                ["Plugin.Api.Rest.ApiKey.Placeholder"] = "Click 'Generate API key' to create one",
                ["Plugin.Api.Rest.ApiKey.Help"] = "Leave the field empty to keep the current key. Treat it like a password: anyone holding it can modify the catalog, orders and customers.",
                ["Plugin.Api.Rest.RateLimitPerMinute"] = "Requests per minute",
                ["Plugin.Api.Rest.RateLimitPerMinute.Help"] = "Maximum number of API requests a single client may send per minute. Set to 0 to disable the limit.",
                ["Plugin.Api.Rest.RequireApiKeyForReads"] = "Require API key for reads",
                ["Plugin.Api.Rest.RequireApiKeyForReads.Help"] = "When enabled, the read endpoints also require the API key. Leave disabled to browse the catalog anonymously, but note that customer and order reads are public as well."
            });

            // add plugin settings or DB initialization here if needed
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourcesAsync("Plugin.Api.Rest");

            // cleanup plugin data if necessary
            await base.UninstallAsync();
        }

        #endregion
    }
}

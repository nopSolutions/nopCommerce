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
                ["Plugin.Api.Rest.OpenSwaggerJson"] = "Open Swagger JSON"
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

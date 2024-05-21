using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Nop.Core;

namespace AbcWarehouse.Plugin.Misc.Redirect
{
    public class RedirectPlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        public RedirectPlugin(
            IWebHelper webHelper,
            ILocalizationService localizationService
        )
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Redirect/Configure";
        }

        public override async System.Threading.Tasks.Task InstallAsync()
        {
            await UpdateLocales();

            await base.InstallAsync();
        }

        public override async System.Threading.Tasks.Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourcesAsync(RedirectLocales.Base);

            await base.UninstallAsync();
        }

        private async System.Threading.Tasks.Task UpdateLocales()
        {
            await _localizationService.AddLocaleResourceAsync(
                new Dictionary<string, string>
                {
                    [RedirectLocales.IsDebugMode] = "Debug Mode",
                    [RedirectLocales.IsDebugModeHint] = "Logs detailed information, useful for debugging issues."
                }
            );
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace AbcWarehouse.Plugin.Widgets.SLI
{
    public class SLIPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;

        public SLIPlugin(
            ILocalizationService localizationService,
            ISettingService settingService)
        {
            _localizationService = localizationService;
            _settingService = settingService;
        }

        public bool HideInWidgetList => false;

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "SLI";
        }

        public System.Threading.Tasks.Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { });
        }

        public override async System.Threading.Tasks.Task InstallAsync()
        {
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                [SLILocaleKeys.ActionUrl] = "Action URL",
                [SLILocaleKeys.ActionUrlHint] = "The URL to POST to, should look like '//appliances.STORE.com'",
                [SLILocaleKeys.CookieName] = "Cookie Name",
                [SLILocaleKeys.CookieNameHint] = "Cookie name used for the Clearance stores.",
            });

            await base.InstallAsync();
        }

        public override async System.Threading.Tasks.Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourcesAsync(SLILocaleKeys.Base);
            await _settingService.DeleteSettingAsync<SLISettings>();

            await base.UninstallAsync();
        }
    }
}

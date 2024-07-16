using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Task = System.Threading.Tasks.Task;
using Nop.Services.Configuration;
using Nop.Data;
using Nop.Core.Domain.Catalog;
using System.Linq;
using Nop.Services.Common;
using Nop.Services.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Seo;

namespace AbcWarehouse.Plugin.Widgets.Flixmedia
{
    public class FlixmediaPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly FlixmediaSettings _settings; 
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public FlixmediaPlugin(
            FlixmediaSettings settings,
            ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper
        )
        {
            _settings = settings;
            _settingService = settingService;
            _localizationService = localizationService;
            _webHelper = webHelper;
        }

        public bool HideInWidgetList => false;

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Flixmedia/Configure";
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsFlixmedia";
        }

        public System.Threading.Tasks.Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            {
                _settings.WidgetZone,
                PublicWidgetZones.ProductDetailsBottom
            });
        }

        public override async Task InstallAsync()
        {
            await AddLocalesAsync();
            await _settingService.SaveSettingAsync(FlixmediaSettings.DefaultValues());

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<FlixmediaSettings>();
            await _localizationService.DeleteLocaleResourcesAsync(FlixmediaLocales.Base);

            await base.UninstallAsync();
        }

        public override async Task UpdateAsync(string currentVersion, string targetVersion)
        {
            await AddLocalesAsync();

            await base.UpdateAsync(currentVersion, targetVersion);
        }

        private async Task AddLocalesAsync()
        {
            await _localizationService.AddLocaleResourceAsync(
                new Dictionary<string, string>
                {
                    [FlixmediaLocales.FlixID] = "Flix ID",
                    [FlixmediaLocales.WidgetZone] = "Widget Zone",
                }
            );
        }
    }
}

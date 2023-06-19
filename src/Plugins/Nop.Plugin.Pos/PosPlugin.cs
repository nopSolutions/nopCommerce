using Nop.Plugin.Pos.Components;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Pos
{
    public class PosPlugin : BasePlugin , IPlugin , IWidgetPlugin
    {
        private readonly ILocalizationService _localizationService;

        public PosPlugin(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

      
        public override async Task InstallAsync()
        {
            base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Pos");
            base.UninstallAsync();
        }

        public bool HideInWidgetList => false;

        public Type GetWidgetViewComponent(string widgetZone)
        {
                return typeof(PosViewComponent);
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { 
                PublicWidgetZones.ProductPriceBottom ,
                PublicWidgetZones.ProductBoxAddinfoAfter
            });
        }
    }
}

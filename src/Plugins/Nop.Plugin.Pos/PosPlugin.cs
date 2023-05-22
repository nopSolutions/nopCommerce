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

        //public override string GetConfigurationPageUrl()
        //{
        //    return "Plugins/Pos/Configure";
        //}

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
            if (widgetZone.Equals(PublicWidgetZones.ProductPriceBottom) || widgetZone.Equals(PublicWidgetZones.ProductSearchPageBeforeResults) ||
                widgetZone.Equals(PublicWidgetZones.HomepageBeforeProducts) || widgetZone.Equals(PublicWidgetZones.ManufacturerDetailsBeforeProductList) || widgetZone.Equals(PublicWidgetZones.CategoryDetailsBeforeFilters) ||
                widgetZone.Equals(PublicWidgetZones.ProductBoxAddinfoAfter))
            {
                return typeof(PosViewComponent);

            }
            else
            {
                return typeof(UserdetailsViewComponent);

            }
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { 
                PublicWidgetZones.ProductPriceBottom ,
                //PublicWidgetZones.ProductSearchPageBeforeResults,
                PublicWidgetZones.HeaderAfter,
                //PublicWidgetZones.HomepageBeforeProducts,
                //PublicWidgetZones.ManufacturerDetailsBeforeProductList,
                //PublicWidgetZones.CategoryDetailsBeforeFilters,
                PublicWidgetZones.ProductBoxAddinfoAfter
                //PublicWidgetZones.HeaderMenuAfter
                //AdminWidgetZones.MenuBefore
              
            });
        }
    }
}

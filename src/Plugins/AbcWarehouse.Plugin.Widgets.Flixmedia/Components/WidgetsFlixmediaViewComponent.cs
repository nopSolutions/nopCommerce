using Nop.Services.Logging;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Core;
using System.Threading.Tasks;
using Nop.Web.Framework.Infrastructure;
using AbcWarehouse.Plugin.Widgets.Flixmedia.Models;
using Nop.Web.Models.Catalog;
using System.Linq;
using Nop.Services.Catalog;

namespace AbcWarehouse.Plugin.Widgets.Flixmedia.Components
{
    public class WidgetsFlixmediaViewComponent : NopViewComponent
    {
        private readonly FlixmediaSettings _settings;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;

        public WidgetsFlixmediaViewComponent(
            FlixmediaSettings settings,
            ILogger logger,
            IWebHelper webHelper
        )
        {
            _settings = settings;
            _logger = logger;
            _webHelper = webHelper;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            if (!_settings.IsValid())
            {
                await _logger.ErrorAsync("Widgets.Flixmedia: Flix ID is required to be " +
                              "set to enable product syndication.");
                return Content("");
            }

            if (widgetZone == _settings.WidgetZone)
            {
                return View("~/Plugins/Widgets.Flixmedia/Views/ContentDisplay.cshtml");
            }
            if (widgetZone == PublicWidgetZones.ProductDetailsBottom)
            {
                var productDetailsModel = additionalData as ProductDetailsModel;
                var model = new ScriptModel()
                {
                    Id = _settings.FlixID,
                    Brand = productDetailsModel.ProductManufacturers.FirstOrDefault()?.Name ?? "",
                    Sku = productDetailsModel.Sku
                };
                return View("~/Plugins/Widgets.Flixmedia/Views/Script.cshtml", model);
            }

            await _logger.ErrorAsync($"Widgets.Flixmedia: No view provided for widget zone {widgetZone}");
            return Content("");
        }
    }
}
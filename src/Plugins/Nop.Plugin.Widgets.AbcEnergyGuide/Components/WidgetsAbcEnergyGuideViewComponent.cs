using System;
using Nop.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.AbcEnergyGuide.Services;
using SevenSpikes.Nop.Plugins.NopQuickTabs.Models;
using Nop.Services.Logging;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AbcBonusBundle.Components
{
    public class WidgetsAbcEnergyGuideViewComponent : NopViewComponent
    {
        private readonly IProductEnergyGuideService _productEnergyGuideService;

        private readonly ILogger _logger;

        public WidgetsAbcEnergyGuideViewComponent(
            IProductEnergyGuideService productEnergyGuideService,
            ILogger logger
        )
        {
            _productEnergyGuideService = productEnergyGuideService;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, TabUIModel additionalData = null)
        {
            if (additionalData == null)
            {
                await _logger.ErrorAsync("Widgets.AbcEnergyGuide: TabUIModel not passed to ViewComponent, skipping display.");
                return Content("");
            }

            var productId = additionalData.ProductId;
            var energyGuide = _productEnergyGuideService.GetEnergyGuideByProductId(productId);
            if (energyGuide == null)
            {
                return Content("");
            }

            return View("~/Plugins/Widgets.AbcEnergyGuide/Views/Display.cshtml", energyGuide.EnergyGuideUrl);
        }
    }
}
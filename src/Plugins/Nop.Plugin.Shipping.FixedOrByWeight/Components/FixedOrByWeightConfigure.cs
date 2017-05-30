
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Shipping.FixedOrByWeight;
using Nop.Plugin.Shipping.FixedOrByWeight.Models;
using Nop.Services.Localization;

namespace Nop.Plugin.Shipping.FixedOrByWeight.Components
{
    [ViewComponent(Name = "FixedOrByWeightConfigure")]
    public class FixedOrByWeightConfigureViewComponent : ViewComponent
    {
        private readonly FixedOrByWeightSettings _fixedOrByWeightSettings;

        public FixedOrByWeightConfigureViewComponent(FixedOrByWeightSettings fixedOrByWeightSettings)
        {
            this._fixedOrByWeightSettings = fixedOrByWeightSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new ConfigurationModel
            {
                LimitMethodsToCreated = _fixedOrByWeightSettings.LimitMethodsToCreated,
                ShippingByWeightEnabled = _fixedOrByWeightSettings.ShippingByWeightEnabled
            };
            return View("~/Plugins/Shipping.FixedOrByWeight/Views/Configure.cshtml", model);
        }
    }
}

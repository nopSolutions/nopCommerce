using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Shipping.AustraliaPost.Models;

namespace Nop.Plugin.Shipping.AustraliaPost.Components
{
    [ViewComponent(Name = "AustraliaPostConfigure")]
    public class AustraliaPostConfigureViewComponent : ViewComponent
    {
        private readonly AustraliaPostSettings _australiaPostSettings;

        public AustraliaPostConfigureViewComponent(AustraliaPostSettings australiaPostSettings)
        {
            this._australiaPostSettings = australiaPostSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new AustraliaPostShippingModel()
            {
                ApiKey = _australiaPostSettings.ApiKey,
                AdditionalHandlingCharge = _australiaPostSettings.AdditionalHandlingCharge
            };

            return View("~/Plugins/Shipping.AustraliaPost/Views/Configure.cshtml", model);
        }
    }
}

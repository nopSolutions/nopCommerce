using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.Zapper.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Zapper.Components
{
    [ViewComponent(Name = "PaymentZapper")]
    public class PaymentZapperViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            PaymentInfoModel model = new PaymentInfoModel
            {
                MerchantId = "9639",
                SiteId = "9649",
                Amount = 10.00M,
                MerchantReference = "Ref01"
            };
           
            return View("~/Plugins/Payments.Zapper/Views/PaymentInfo.cshtml", model);
        }
    }
}

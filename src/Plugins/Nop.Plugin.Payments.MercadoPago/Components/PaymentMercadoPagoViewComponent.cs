using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.MercadoPago.Components
{
    [ViewComponent(Name = "PaymentMercadoPago")]
    public class PaymentMercadoPagoViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.MercadoPago/Views/PaymentInfo.cshtml");
        }
    }
}

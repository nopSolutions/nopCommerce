using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace NopBrasil.Plugin.Payments.PagSeguro.Components
{
    [ViewComponent(Name = "PaymentPagSeguro")]
    public class PaymentPagSeguroViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.PagSeguro/Views/PaymentInfo.cshtml");
        }
    }
}

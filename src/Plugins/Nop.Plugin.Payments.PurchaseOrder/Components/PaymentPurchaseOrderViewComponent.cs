using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.PurchaseOrder.Models;

namespace Nop.Plugin.Payments.PurchaseOrder.Components
{
    [ViewComponent(Name = "PaymentPurchaseOrder")]
    public class PaymentPurchaseOrderViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new PaymentInfoModel();
            //set postback values (we cannot access "Form" with "GET" requests)
            if (this.Request.Method != WebRequestMethods.Http.Get)
            {
                model.PurchaseOrderNumber = this.HttpContext.Request.Form["PurchaseOrderNumber"];
            }

            return View("~/Plugins/Payments.PurchaseOrder/Views/PaymentInfo.cshtml", model);
        }
    }
}

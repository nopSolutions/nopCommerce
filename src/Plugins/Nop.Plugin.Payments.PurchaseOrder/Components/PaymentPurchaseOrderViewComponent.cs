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
            //set postback values
            var model = new PaymentInfoModel
            {
                PurchaseOrderNumber = this.HttpContext?.Request?.Form?["PurchaseOrderNumber"]
            };

            return View("~/Plugins/Payments.PurchaseOrder/Views/PaymentInfo.cshtml", model);
        }
    }
}

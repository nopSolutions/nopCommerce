using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.PurchaseOrder.Models;
using Nop.Web.Framework.Components;


namespace Nop.Plugin.Payments.PurchaseOrder.Components
{
    [ViewComponent(Name = "PaymentPurchaseOrder")]
    public class PaymentPurchaseOrderViewComponent : NopViewComponent
    {

        public IViewComponentResult Invoke()
        {
            var model = new PaymentInfoModel();
            //set postback values (we cannot access "Form" with "GET" requests)
            if (Request.Method != WebRequestMethods.Http.Get)
            {
                model.PurchaseOrderNumber = HttpContext.Request.Form["PurchaseOrderNumber"];
                //if (model.PurchaseOrderNumber.Trim().ToString() == "")
                //{
                //    EngineContext.Current.Resolve<ILogger>()?.Error(exception.Message, exception);
                //}
            }

            return View("~/Plugins/Payments.PurchaseOrder/Views/PaymentInfo.cshtml", model);
        }
    }
}

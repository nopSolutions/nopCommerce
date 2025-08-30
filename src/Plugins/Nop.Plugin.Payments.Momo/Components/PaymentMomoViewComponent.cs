using Microsoft.AspNetCore.Mvc;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.Momo.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Momo.Components;

public class PaymentMomoViewComponent : NopViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = new PaymentInfoModel()
        {
            PhoneNumber = string.Empty,
        };

        //set postback values (we cannot access "Form" with "GET" requests)
        if (!Request.IsGetRequest())
        {
            var form = await Request.ReadFormAsync();

            model.PhoneNumber = form["PhoneNumber"];
        }

        return View("~/Plugins/Payments.Momo/Views/PaymentInfo.cshtml", model);
    }
}
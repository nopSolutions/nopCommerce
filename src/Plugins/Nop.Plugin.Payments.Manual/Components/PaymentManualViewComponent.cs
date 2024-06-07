using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.Manual.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Manual.Components;

public class PaymentManualViewComponent : NopViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = new PaymentInfoModel()
        {
            CreditCardTypes = new List<SelectListItem>
            {
                new() { Text = "Visa", Value = "visa" },
                new() { Text = "Master card", Value = "MasterCard" },
                new() { Text = "Discover", Value = "Discover" },
                new() { Text = "Amex", Value = "Amex" },
            }
        };

        //years
        for (var i = 0; i < 15; i++)
        {
            var year = (DateTime.Now.Year + i).ToString();
            model.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
        }

        //months
        for (var i = 1; i <= 12; i++)
        {
            model.ExpireMonths.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString(), });
        }

        //set postback values (we cannot access "Form" with "GET" requests)
        if (!Request.IsGetRequest())
        {
            var form = await Request.ReadFormAsync();

            model.CardholderName = form["CardholderName"];
            model.CardNumber = form["CardNumber"];
            model.CardCode = form["CardCode"];
            var selectedCcType = model.CreditCardTypes.FirstOrDefault(x => x.Value.Equals(form["CreditCardType"], StringComparison.InvariantCultureIgnoreCase));
            if (selectedCcType != null)
                selectedCcType.Selected = true;
            var selectedMonth = model.ExpireMonths.FirstOrDefault(x => x.Value.Equals(form["ExpireMonth"], StringComparison.InvariantCultureIgnoreCase));
            if (selectedMonth != null)
                selectedMonth.Selected = true;
            var selectedYear = model.ExpireYears.FirstOrDefault(x => x.Value.Equals(form["ExpireYear"], StringComparison.InvariantCultureIgnoreCase));
            if (selectedYear != null)
                selectedYear.Selected = true;
        }

        return View("~/Plugins/Payments.Manual/Views/PaymentInfo.cshtml", model);
    }
}
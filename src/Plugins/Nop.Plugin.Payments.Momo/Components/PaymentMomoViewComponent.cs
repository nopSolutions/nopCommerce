using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.Momo.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Momo.Components;

public class PaymentMomoViewComponent : NopViewComponent
{
    private readonly IWorkContext _workContext;
    private readonly MomoPaymentSettings _momoPaymentSettings;
    private readonly ILocalizationService _localizationService;

    public PaymentMomoViewComponent(IWorkContext workContext, MomoPaymentSettings momoPaymentSettings, ILocalizationService localizationService)
    {
        _workContext = workContext;
        _localizationService = localizationService;
        _momoPaymentSettings = momoPaymentSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = new PaymentInfoModel()
        {
            PhoneNumber = string.Empty,
            PhoneNumberRequired = true,
            PaymentNote = await _localizationService.GetResourceAsync("Plugins.Payments.Momo.PaymentNote") ?? string.Empty
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
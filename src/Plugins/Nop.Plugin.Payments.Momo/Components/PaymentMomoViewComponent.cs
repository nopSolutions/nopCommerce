using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.Momo.Models;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Momo.Components;

public class PaymentMomoViewComponent : NopViewComponent
{
    private readonly IWorkContext _workContext;
    private readonly MomoPaymentSettings _momoPaymentSettings;
    private readonly ILocalizationService _localizationService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IOrderTotalCalculationService _orderTotalCalculationService;
    private readonly IStoreContext _storeContext;

    public PaymentMomoViewComponent(
        IWorkContext workContext, 
        MomoPaymentSettings momoPaymentSettings, 
        ILocalizationService localizationService,
        IShoppingCartService shoppingCartService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IStoreContext storeContext)
    {
        _workContext = workContext;
        _localizationService = localizationService;
        _momoPaymentSettings = momoPaymentSettings;
        _shoppingCartService = shoppingCartService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _storeContext = storeContext;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var model = new PaymentInfoModel()
        {
            PhoneNumber = string.Empty,
            PhoneNumberRequired = true,
            PaymentNote = await _localizationService.GetResourceAsync("Plugins.Payments.Momo.PaymentNote") ?? string.Empty,
            CustomerEmail = customer?.Email ?? "N/A"
        };

        // Get order total
        var store = await _storeContext.GetCurrentStoreAsync();
        var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
        
        if (shoppingCart.Any())
        {
            var (shoppingCartTotal, _, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(shoppingCart);
            model.OrderTotal = shoppingCartTotal?.ToString("C") ?? "N/A";
        }
        else
        {
            model.OrderTotal = "N/A";
        }

        //set postback values (we cannot access "Form" with "GET" requests)
        if (Request.IsGetRequest()) return View("~/Plugins/Payments.Momo/Views/PaymentInfo.cshtml", model);
        var form = await Request.ReadFormAsync();

        model.PhoneNumber = form["PhoneNumber"];

        return View("~/Plugins/Payments.Momo/Views/PaymentInfo.cshtml", model);
    }
}
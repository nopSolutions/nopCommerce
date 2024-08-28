using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.AmazonPay.Enums;
using Nop.Plugin.Payments.AmazonPay.Services;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.AmazonPay.Components;

/// <summary>
/// Represents the view component to display plugin payment info
/// </summary>
public class PaymentInfoViewComponent : NopViewComponent
{
    #region Fields

    private readonly AmazonPayApiService _amazonPayApiService;
    private readonly AmazonPayCheckoutService _amazonPayCheckoutService;
    private readonly AmazonPayCustomerService _amazonPayCustomerService;
    private readonly AmazonPaySettings _amazonPaySettings;
    private readonly IHttpContextAccessor _httpContextAccessor;

    #endregion

    #region Ctor

    public PaymentInfoViewComponent(AmazonPayApiService amazonPayApiService,
        AmazonPayCheckoutService amazonPayCheckoutService,
        AmazonPayCustomerService amazonPayCustomerService,
        AmazonPaySettings amazonPaySettings,
        IHttpContextAccessor httpContextAccessor)
    {
        _amazonPayApiService = amazonPayApiService;
        _amazonPayCheckoutService = amazonPayCheckoutService;
        _amazonPayCustomerService = amazonPayCustomerService;
        _amazonPaySettings = amazonPaySettings;
        _httpContextAccessor = httpContextAccessor;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        //ensure that plugin is active and configured
        if (!await _amazonPayApiService.IsActiveAndConfiguredAsync())
            return Content(string.Empty);

        var values = _httpContextAccessor.HttpContext?.Request.RouteValues;

        if (values != null && (values["controller"]?.Equals("Customer") ?? false))
        {
            var signInModel = await _amazonPayCustomerService.GetSignInModelAsync();
            if (string.IsNullOrEmpty(signInModel?.Signature))
                return Content(string.Empty);

            return View("~/Plugins/Payments.AmazonPay/Views/SignIn.cshtml", signInModel);
        }

        if (values != null && values.ContainsKey("controller") && (values["controller"]?.Equals("Checkout") ?? false))
        {
            if (!_amazonPaySettings.ButtonPlacement.Contains(ButtonPlacement.PaymentMethod))
                return Content(string.Empty);

            var model = await _amazonPayCheckoutService.GetPaymentInfoModelAsync(ButtonPlacement.PaymentMethod);
            if (model is null)
                return Content(string.Empty);

            return View("~/Plugins/Payments.AmazonPay/Views/PaymentInfo.cshtml", model);
        }

        if (!_amazonPaySettings.ButtonPlacement.Contains(ButtonPlacement.Cart))
            return Content(string.Empty);

        var cartModel = await _amazonPayCheckoutService.GetPaymentInfoModelAsync(ButtonPlacement.Cart);
        if (cartModel is null)
            return Content(string.Empty);

        return View("~/Plugins/Payments.AmazonPay/Views/PaymentInfo.cshtml", cartModel);
    }

    #endregion
}
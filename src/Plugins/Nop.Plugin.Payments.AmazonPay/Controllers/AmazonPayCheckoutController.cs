using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.AmazonPay.Enums;
using Nop.Plugin.Payments.AmazonPay.Services;
using Nop.Services.Messages;
using Nop.Web.Controllers;

namespace Nop.Plugin.Payments.AmazonPay.Controllers;

[AutoValidateAntiforgeryToken]
public class AmazonPayCheckoutController(AmazonPayCheckoutService amazonPayCheckoutService,
        INotificationService notificationService) : BasePublicController
{
    #region Methods

    public async Task<IActionResult> Confirm()
    {
        try
        {
            await amazonPayCheckoutService.ReadAndSaveCheckoutSessionIdAsync();

            //validation
            var cart = await amazonPayCheckoutService.GetCartAsync();
            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (!await amazonPayCheckoutService.IsAllowedToCheckoutAsync())
                return Challenge();

            //model
            var model = await amazonPayCheckoutService.PrepareConfirmOrderModelAsync(cart);

            return View("~/Plugins/Payments.AmazonPay/Views/Confirm.cshtml", model);
        }
        catch (Exception exception)
        {
            notificationService.ErrorNotification(exception.Message);

            return RedirectToRoute("ShoppingCart");
        }
    }

    [HttpPost, ActionName("Confirm")]
    public async Task<IActionResult> ConfirmOrder(string shippingoption)
    {
        try
        {
            //validation
            var cart = await amazonPayCheckoutService.GetCartAsync();
            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (!await amazonPayCheckoutService.IsAllowedToCheckoutAsync())
                return Challenge();

            if (!await amazonPayCheckoutService.ShoppingCartRequiresShippingAsync(cart))
                await amazonPayCheckoutService.SetShippingMethodToNullAsync();
            else
            {
                var success = await amazonPayCheckoutService.SetShippingMethodAsync(cart, shippingoption);

                if (!success)
                    return RedirectToAction("Confirm");
            }

            var scWarnings = await amazonPayCheckoutService.GetShoppingCartWarningsAsync(cart);
            if (!string.IsNullOrEmpty(scWarnings))
            {
                notificationService.WarningNotification(string.Join("<br />", scWarnings));

                return RedirectToRoute("ShoppingCart");
            }

            var url = await amazonPayCheckoutService.UpdateCheckoutSessionAsync();

            return Redirect(url);
        }
        catch (Exception exception)
        {
            notificationService.ErrorNotification(exception.Message);

            return RedirectToRoute("ShoppingCart");
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetPaymentInfo(int placement)
    {
        var (payload, signature, amount) = await amazonPayCheckoutService.CreateCheckoutSessionAsync((ButtonPlacement)placement);

        if (string.IsNullOrEmpty(payload))
            return ErrorJson($"{AmazonPayDefaults.PluginSystemName} error: Payload creation failed");

        if (string.IsNullOrEmpty(signature))
            return ErrorJson($"{AmazonPayDefaults.PluginSystemName} error: Signature validation failed");

        return Json(new { payload, signature, amount });
    }

    public async Task<IActionResult> Completed()
    {
        try
        {
            var cart = await amazonPayCheckoutService.GetCartAsync();
            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            var (order, warnings, model) = await amazonPayCheckoutService.CompleteCheckoutAsync();

            if (order == null)
            {
                if (warnings.Any())
                    notificationService.WarningNotification(string.Join("<br />", warnings));

                return RedirectToRoute("ShoppingCart");
            }

            return View("~/Plugins/Payments.AmazonPay/Views/Completed.cshtml", model);
        }
        catch (Exception exception)
        {
            notificationService.ErrorNotification(exception.Message);

            return RedirectToRoute("ShoppingCart");
        }
    }

    #endregion
}
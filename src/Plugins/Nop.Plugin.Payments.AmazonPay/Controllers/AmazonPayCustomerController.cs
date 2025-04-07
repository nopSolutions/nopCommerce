using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.AmazonPay.Services;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.AmazonPay.Controllers;

[AutoValidateAntiforgeryToken]
public class AmazonPayCustomerController(AmazonPayCustomerService amazonPayCustomerService) : BasePaymentController
{
    #region Methods

    public async Task<IActionResult> SignIn()
    {
        return await amazonPayCustomerService.SignInAsync();
    }

    [HttpPost]
    public async Task<IActionResult> AssociateOrCreateAccount(string buyerId, string buyerEmail, string buyerName)
    {
        if (await amazonPayCustomerService.AssociateOrCreateCustomerAsync(buyerId, buyerEmail, buyerName))
            return Json(new { Redirect = true });

        return Json(new { Status = "Ok!" });
    }

    [HttpPost]
    public async Task<IActionResult> SignOut(string buyerId)
    {
        await amazonPayCustomerService.SignOutAsync(buyerId);

        return Json(new { Status = "Ok!" });
    }

    #endregion
}
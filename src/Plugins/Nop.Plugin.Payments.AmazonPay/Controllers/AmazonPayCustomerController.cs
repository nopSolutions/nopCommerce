using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.AmazonPay.Services;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.AmazonPay.Controllers;

[AutoValidateAntiforgeryToken]
public class AmazonPayCustomerController : BasePaymentController
{
    #region Fields

    private readonly AmazonPayCustomerService _amazonPayCustomerService;

    #endregion

    #region Ctor

    public AmazonPayCustomerController(AmazonPayCustomerService amazonPayCustomerService)
    {
        _amazonPayCustomerService = amazonPayCustomerService;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> SignIn()
    {
        return await _amazonPayCustomerService.SignInAsync();
    }

    [HttpPost]
    public async Task<IActionResult> AssociateOrCreateAccount(string buyerId, string buyerEmail, string buyerName)
    {
        if (await _amazonPayCustomerService.AssociateOrCreateCustomerAsync(buyerId, buyerEmail, buyerName))
            return Json(new { Redirect = true });

        return Json(new { Status = "Ok!" });
    }

    [HttpPost]
    public async Task<IActionResult> SignOut(string buyerId)
    {
        await _amazonPayCustomerService.SignOutAsync(buyerId);

        return Json(new { Status = "Ok!" });
    }

    #endregion
}
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.PayPalCommerce.Services;

namespace Nop.Plugin.Payments.PayPalCommerce.Controllers;

public class PayPalCommerceWebhookController(PayPalCommerceServiceManager serviceManager,
        PayPalCommerceSettings settings) : Controller
{
    #region Methods

    [HttpPost]
    public async Task<IActionResult> WebhookHandler()
    {
        await serviceManager.HandleWebhookAsync(settings, Request);
        return Ok();
    }

    #endregion
}
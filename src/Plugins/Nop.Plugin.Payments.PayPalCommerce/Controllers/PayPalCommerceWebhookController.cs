using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.PayPalCommerce.Services;

namespace Nop.Plugin.Payments.PayPalCommerce.Controllers;

public class PayPalCommerceWebhookController : Controller
{
    #region Fields

    protected readonly PayPalCommerceSettings _settings;
    protected readonly ServiceManager _serviceManager;

    #endregion

    #region Ctor

    public PayPalCommerceWebhookController(PayPalCommerceSettings settings,
        ServiceManager serviceManager)
    {
        _settings = settings;
        _serviceManager = serviceManager;
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> WebhookHandler()
    {
        await _serviceManager.HandleWebhookAsync(_settings, Request);
        return Ok();
    }

    #endregion
}
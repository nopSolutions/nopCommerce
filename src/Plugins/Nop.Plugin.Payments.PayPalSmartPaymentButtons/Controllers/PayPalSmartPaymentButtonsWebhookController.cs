using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Services;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Controllers
{
    public class PayPalSmartPaymentButtonsWebhookController : Controller
    {
        #region Fields

        private readonly PayPalSmartPaymentButtonsSettings _settings;
        private readonly ServiceManager _serviceManager;

        #endregion

        #region Ctor

        public PayPalSmartPaymentButtonsWebhookController(PayPalSmartPaymentButtonsSettings settings,
            ServiceManager serviceManager)
        {
            _settings = settings;
            _serviceManager = serviceManager;
        }

        #endregion

        #region Methods

        [HttpPost]
        public IActionResult WebhookHandler()
        {
            _serviceManager.HandleWebhook(_settings, Request);
            return Ok();
        }

        #endregion
    }
}
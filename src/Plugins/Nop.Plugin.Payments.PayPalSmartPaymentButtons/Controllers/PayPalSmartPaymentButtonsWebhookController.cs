using System.Threading.Tasks;
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
        public async Task<IActionResult> WebhookHandler()
        {
            await _serviceManager.HandleWebhookAsync(_settings, Request);
            return Ok();
        }

        #endregion
    }
}
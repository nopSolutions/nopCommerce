using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Brevo.Services;

namespace Nop.Plugin.Misc.Brevo.Controllers
{
    public class BrevoWebhookController : Controller
    {
        #region Fields

        private readonly BrevoManager _brevoEmailManager;

        #endregion

        #region Ctor

        public BrevoWebhookController(BrevoManager brevoEmailManager)
        {
            _brevoEmailManager = brevoEmailManager;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> UnsubscribeWebHook()
        {
            await _brevoEmailManager.HandleWebhookAsync(Request);
            return Ok();
        }

        #endregion
    }
}

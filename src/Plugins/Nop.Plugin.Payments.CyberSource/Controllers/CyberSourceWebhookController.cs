using Microsoft.AspNetCore.Mvc;

namespace Nop.Plugin.Payments.CyberSource.Controllers
{
    public class CyberSourceWebhookController : Controller
    {
        #region Methods

        public IActionResult PayerRedirect()
        {
            return Ok();
        }

        #endregion
    }
}
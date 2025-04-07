using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Brevo.Services;

namespace Nop.Plugin.Misc.Brevo.Controllers;

public class BrevoWebhookController(BrevoManager brevoEmailManager) : Controller
{
    #region Methods

    [HttpPost]
    public async Task<IActionResult> UnsubscribeWebHook()
    {
        await brevoEmailManager.HandleWebhookAsync(Request);
        return Ok();
    }

    #endregion
}
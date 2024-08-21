using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Tax.Avalara.Services;

namespace Nop.Plugin.Tax.Avalara.Controllers;

public class AvalaraWebhookController : Controller
{
    #region Fields

    protected readonly AvalaraTaxManager _avalaraTaxManager;

    #endregion

    #region Ctor

    public AvalaraWebhookController(AvalaraTaxManager avalaraTaxManager)
    {
        _avalaraTaxManager = avalaraTaxManager;
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> ItemClassificationWebhook()
    {
        await _avalaraTaxManager.HandleItemClassificationWebhookAsync(Request);
        return Ok();
    }

    #endregion
}
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Zettle.Services;

namespace Nop.Plugin.Misc.Zettle.Controllers
{
    public class ZettleWebhookController : Controller
    {
        #region Fields

        protected readonly ZettleService _zettleService;

        #endregion

        #region Ctor

        public ZettleWebhookController(ZettleService zettleService)
        {
            _zettleService = zettleService;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> Webhook()
        {
            await _zettleService.HandleWebhookAsync(Request);
            return Ok();
        }

        #endregion
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Shipping.EasyPost.Services;

namespace Nop.Plugin.Shipping.EasyPost.Controllers
{
    public class EasyPostPublicController : Controller
    {
        #region Fields

        private readonly EasyPostService _easyPostService;

        #endregion

        #region Ctor

        public EasyPostPublicController(EasyPostService easyPostService)
        {
            _easyPostService = easyPostService;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> Webhook()
        {
            await _easyPostService.HandleWebhookAsync(Request);
            return Ok();
        }

        #endregion
    }
}
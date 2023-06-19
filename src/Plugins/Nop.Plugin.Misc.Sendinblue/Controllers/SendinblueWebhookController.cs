using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Sendinblue.Services;

namespace Nop.Plugin.Misc.Sendinblue.Controllers
{
    public class SendinblueWebhookController : Controller
    {

        #region Fields

        private readonly SendinblueManager _sendinblueEmailManager;

        #endregion

        #region Ctor

        public SendinblueWebhookController(SendinblueManager sendinblueEmailManager)
        {
            _sendinblueEmailManager = sendinblueEmailManager;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> UnsubscribeWebHook()
        {
            await _sendinblueEmailManager.HandleWebhookAsync(Request);
            return Ok();
        }

        #endregion
    }
}

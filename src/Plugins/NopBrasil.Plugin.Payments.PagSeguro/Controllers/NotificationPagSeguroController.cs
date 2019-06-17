using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NopBrasil.Plugin.Payments.PagSeguro.Controllers
{
    [AllowAnonymous]
    public class NotificationPagSeguroController : Controller
    {
        public IActionResult GetPushNotification()
        {
            return Ok("Trace done");
        }

        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult PushNotification([FromForm] string notificationCode, [FromForm] string notificationType)
        {
            Trace.TraceInformation(notificationCode);
            Trace.TraceInformation(notificationType);

            return Ok();
        }
    }
}

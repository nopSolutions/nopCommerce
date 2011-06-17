using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    public class PrivateMessagesController : BaseNopController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendPM(int customerId)
        {
            return Content("TODO send private message to customerId: " + customerId);
        }

        public ActionResult ViewPM(int privateMessageId)
        {
            return Content("TODO view private message, privateMessageId: " + privateMessageId);
        }
    }
}

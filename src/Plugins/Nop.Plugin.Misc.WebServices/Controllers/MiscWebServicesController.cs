using System.Web.Mvc;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.WebServices.Controllers
{
    [AdminAuthorize]
    public class MiscWebServicesController : BasePluginController
    {
        public ActionResult Configure()
        {
            return View("~/Plugins/Misc.WebServices/Views/MiscWebServices/Configure.cshtml");
        }
    }
}

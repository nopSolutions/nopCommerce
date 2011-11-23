using System.Web.Mvc;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.WebServices.Controllers
{
    [AdminAuthorize]
    public class MiscWebServicesController : Controller
    {
        public ActionResult Configure()
        {
            return View("Nop.Plugin.Misc.WebServices.Views.MiscWebServices.Configure");
        }
    }
}

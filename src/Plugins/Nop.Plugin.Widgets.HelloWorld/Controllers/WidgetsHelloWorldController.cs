using System.Web.Mvc;
using Nop.Plugin.Widgets.HelloWorld.Models;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.HelloWorld.Controllers
{
    public class WidgetsHelloWorldController : BasePluginController
    {
        public WidgetsHelloWorldController()
        {
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            return View("~/Plugins/Widgets.HelloWorld/Views/WidgetsHelloWorld/Configure.cshtml");
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            return Configure();
        }

        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            return View("~/Plugins/Widgets.HelloWorld/Views/WidgetsHelloWorld/PublicInfo.cshtml", null);
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.CustomAPI.Components
{
    [ViewComponent(Name = "CustomAPI")]
    public class CustomAPIViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/CustomAPI/Views/TestInfo.cshtml");
        }

    }
}
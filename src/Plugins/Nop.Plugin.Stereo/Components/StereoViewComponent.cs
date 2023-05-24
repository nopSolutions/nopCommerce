using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Stereo.Components
{
    [ViewComponent(Name = "Stereo")]
    public class StereoViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Stereo/Views/OpenStore.cshtml");
        }
    }
}
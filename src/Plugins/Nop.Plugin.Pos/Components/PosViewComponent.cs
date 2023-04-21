using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Pos.Components
{
    [ViewComponent(Name = "Pos")]
    public class PosViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Pos/Views/GoToPos.cshtml");
        }
    }
}
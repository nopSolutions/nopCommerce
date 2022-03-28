using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Components
{
    public class CartSlideoutViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke(string widgetZone, object additionalData = null)
        {
            return View("~/Plugins/Widgets.CartSlideout/Views/Slideout.cshtml");
        }
    }
}

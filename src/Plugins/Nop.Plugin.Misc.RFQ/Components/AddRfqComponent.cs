using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.RFQ.Components;

[ViewComponent(Name = "AddRfq")]
public class AddRfqComponent : NopViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}

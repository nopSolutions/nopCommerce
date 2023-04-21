using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Pos.Components
{
    [ViewComponent(Name = "Userdetails")]
    public class UserdetailsViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Pos/Views/UserDetails.cshtml");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.UserDeatils.Components
{
    [ViewComponent(Name = "Userdetails")]
    public class UserdetailsViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/UserDeatils/Views/UserDetails.cshtml");
        }
    }
}
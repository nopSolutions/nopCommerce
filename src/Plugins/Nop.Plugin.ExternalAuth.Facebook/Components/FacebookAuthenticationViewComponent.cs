using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Plugin.ExternalAuth.Facebook.Components
{
    [ViewComponent(Name = "FacebookAuthentication")]
    public class FacebookAuthenticationViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("~/Plugins/ExternalAuth.Facebook/Views/PublicInfo.cshtml");
        }
    }
}
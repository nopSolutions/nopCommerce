using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}
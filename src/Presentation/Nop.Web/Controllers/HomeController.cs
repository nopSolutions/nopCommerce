using System.Web.Mvc;
using Nop.Web.Framework.Security;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BaseNopController
    {
        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult Index()
        {
            return View();
        }
    }
}

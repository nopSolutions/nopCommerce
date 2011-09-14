using System.Web.Mvc;
using Nop.Web.Framework;

namespace Nop.Web.Controllers
{
    [Compress]
    [StoreLastVisitedPage]
    [CheckAffiliate]
    [StoreClosedAttribute]
    public class BaseNopController : Controller
    {
    }
}

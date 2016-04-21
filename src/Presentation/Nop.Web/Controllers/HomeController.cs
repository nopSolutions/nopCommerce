using System.Web.Mvc;
using Nop.Core.Caching;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;
using Nop.Web.Infrastructure.Cache;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        [NopOutputCache(CacheKeyPrefix = ModelCacheEventConsumer.HOMEPAGE_OUTPUT_PATTERN_KEY, VaryByCustomer = true, DurationInMinutes = CacheProfiles.ShortLivedProfile)]
        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult Index()
        {
            return View();
        }
    }
}

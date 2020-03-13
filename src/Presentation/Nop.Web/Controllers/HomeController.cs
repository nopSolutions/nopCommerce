using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Nop.Core.Caching;
using Nop.Services.Caching.CachingDefaults;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        public HomeController(IStaticCacheManager cacheManager, IMemoryCache manager)
        {
            var c1 = NopBlogsCachingDefaults.BlogCommentsNumberCacheKey.FillCacheKey(1, 2, 3);
            var c2 = NopBlogsCachingDefaults.BlogCommentsNumberCacheKey.FillCacheKey(4, 5, 6);
        }

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}
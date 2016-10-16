using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Nop.Core.Infrastructure;
using Nop.Services.Security;

namespace DataShop.DemoPlugin.Infrastructure
{
    public class MyAuthorization : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var permissionService = EngineContext.Current.Resolve<IPermissionService>();
            if (!permissionService.Authorize(StandardPermissionProvider.ManageProducts))
            {
                return false;
            }
            return base.AuthorizeCore(httpContext);
        }
    }
}

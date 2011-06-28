using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Security;

namespace Nop.Web.Framework.Controllers
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var permissionService = EngineContext.Current.Resolve<IPermissionService>();
            bool result = permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel);
            return result;


            //previous implementation
            //var workContext = EngineContext.Current.Resolve<IWorkContext>();
            //var user = workContext.CurrentCustomer;
            //bool result = user != null && user.IsAdmin();
            //return result;
        }
    }
}

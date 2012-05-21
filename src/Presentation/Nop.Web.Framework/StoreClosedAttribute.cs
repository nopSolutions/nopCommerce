using System;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;

namespace Nop.Web.Framework
{
    public class StoreClosedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            string actionName = filterContext.ActionDescriptor.ActionName;
            if (String.IsNullOrEmpty(actionName))
                return;

            string controllerName = filterContext.Controller.ToString();
            if (String.IsNullOrEmpty(controllerName))
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            var storeInformationSettings = EngineContext.Current.Resolve<StoreInformationSettings>();
            if (storeInformationSettings.StoreClosed &&
                //ensure it's not the Login page
                !(controllerName.Equals("Nop.Web.Controllers.CustomerController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("Login", StringComparison.InvariantCultureIgnoreCase)) &&
                //ensure it's not the Logout page
                !(controllerName.Equals("Nop.Web.Controllers.CustomerController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("Logout", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (storeInformationSettings.StoreClosedAllowForAdmins && EngineContext.Current.Resolve<IWorkContext>().CurrentCustomer.IsAdmin())
                {
                    //do nothing - allow admin access
                }
                else
                {
                    var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                    filterContext.Result = new RedirectResult(webHelper.GetStoreLocation() + "StoreClosed.htm");
                }
            }
        }
    }
}

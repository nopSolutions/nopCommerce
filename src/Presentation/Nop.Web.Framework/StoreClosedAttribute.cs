using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;

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
            if (!storeInformationSettings.StoreClosed)
                return;

            //<controller, action>
            var allowedPages = new List<Tuple<string, string>>();
            //login page
            allowedPages.Add(new Tuple<string, string>("Nop.Web.Controllers.CustomerController", "Login"));
            //logout page
            allowedPages.Add(new Tuple<string, string>("Nop.Web.Controllers.CustomerController", "Logout"));
            //store closed page
            allowedPages.Add(new Tuple<string, string>("Nop.Web.Controllers.CommonController", "EuCookieLawAccept"));
            //the method (AJAX) for accepting EU Cookie law
            allowedPages.Add(new Tuple<string, string>("Nop.Web.Controllers.CommonController", "StoreClosed"));
            //the change language page (request)
            allowedPages.Add(new Tuple<string, string>("Nop.Web.Controllers.CommonController", "SetLanguage"));
            //contact us page
            allowedPages.Add(new Tuple<string, string>("Nop.Web.Controllers.CommonController", "ContactUs"));
            allowedPages.Add(new Tuple<string, string>("Nop.Web.Controllers.CommonController", "ContactUsSend"));
            var isPageAllowed = allowedPages.Any(
                x => controllerName.Equals(x.Item1, StringComparison.InvariantCultureIgnoreCase) &&
                     actionName.Equals(x.Item2, StringComparison.InvariantCultureIgnoreCase));
            if (isPageAllowed)
                return;

            //allow admin access
            if (storeInformationSettings.StoreClosedAllowForAdmins &&
                EngineContext.Current.Resolve<IWorkContext>().CurrentCustomer.IsAdmin())
                return;

            var storeClosedUrl = new UrlHelper(filterContext.RequestContext).RouteUrl("StoreClosed");
            filterContext.Result = new RedirectResult(storeClosedUrl);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Services.Security;

namespace Nop.Web.Framework
{
    public class PublicStoreAllowNavigationAttribute : ActionFilterAttribute
    {
        private static IEnumerable<string> BaseTypes(Type type)
        {
            for (var t = type.BaseType; t != null; t = t.BaseType)
            {
                yield return t.ToString();
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            string actionName = filterContext.ActionDescriptor.ActionName;
            if (String.IsNullOrEmpty(actionName))
                return;

            //we validate all controllers (in case of inheritance)
            var controllerTypes = new List<string>();
            controllerTypes.Add(filterContext.Controller.ToString());
            controllerTypes.AddRange(BaseTypes(filterContext.Controller.GetType()));


            var permissionService = EngineContext.Current.Resolve<IPermissionService>();
            var publicStoreAllowNavigation = permissionService.Authorize(StandardPermissionProvider.PublicStoreAllowNavigation);
            if (publicStoreAllowNavigation)
                return;

            if (//ensure it's not the Login page
                !(controllerTypes.Any(x=> x.Equals("Nop.Web.Controllers.CustomerController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("Login", StringComparison.InvariantCultureIgnoreCase))) &&
                //ensure it's not the Logout page
                !(controllerTypes.Any(x => x.Equals("Nop.Web.Controllers.CustomerController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("Logout", StringComparison.InvariantCultureIgnoreCase))) &&
                //ensure it's not the Register page
                !(controllerTypes.Any(x => x.Equals("Nop.Web.Controllers.CustomerController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("Register", StringComparison.InvariantCultureIgnoreCase))) &&
                !(controllerTypes.Any(x => x.Equals("Nop.Web.Controllers.CustomerController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("RegisterResult", StringComparison.InvariantCultureIgnoreCase))) &&
                //ensure it's not the Password recovery page
                !(controllerTypes.Any(x => x.Equals("Nop.Web.Controllers.CustomerController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("PasswordRecovery", StringComparison.InvariantCultureIgnoreCase))) &&
                !(controllerTypes.Any(x => x.Equals("Nop.Web.Controllers.CustomerController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("PasswordRecoveryConfirm", StringComparison.InvariantCultureIgnoreCase))) &&
                //ensure it's not the Account activation page
                !(controllerTypes.Any(x => x.Equals("Nop.Web.Controllers.CustomerController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("AccountActivation", StringComparison.InvariantCultureIgnoreCase))) &&
                //ensure it's not the Register page
                !(controllerTypes.Any(x => x.Equals("Nop.Web.Controllers.CustomerController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("CheckUsernameAvailability", StringComparison.InvariantCultureIgnoreCase))) &&
                //ensure it's not the GetStatesByCountryId ajax method (can be used during registration)
                !(controllerTypes.Any(x => x.Equals("Nop.Web.Controllers.CountryController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("GetStatesByCountryId", StringComparison.InvariantCultureIgnoreCase))) &&
                //ensure it's not the method (AJAX) for accepting EU Cookie law
                !(controllerTypes.Any(x => x.Equals("Nop.Web.Controllers.CommonController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("EuCookieLawAccept", StringComparison.InvariantCultureIgnoreCase))) &&
                //ensure it's "change language" method
                !(controllerTypes.Any(x => x.Equals("Nop.Web.Controllers.CommonController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("SetLanguage", StringComparison.InvariantCultureIgnoreCase))) &&
                //ensure it's "change currency" method
                !(controllerTypes.Any(x => x.Equals("Nop.Web.Controllers.CommonController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("SetCurrency", StringComparison.InvariantCultureIgnoreCase))) &&
                //ensure it's "change tax" method
                !(controllerTypes.Any(x => x.Equals("Nop.Web.Controllers.CommonController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("SetTaxType", StringComparison.InvariantCultureIgnoreCase))))
            {
                //var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                //var loginPageUrl = webHelper.GetStoreLocation() + "login";
                //var loginPageUrl = new UrlHelper(filterContext.RequestContext).RouteUrl("login");
                //filterContext.Result = new RedirectResult(loginPageUrl);
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}

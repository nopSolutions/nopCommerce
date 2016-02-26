using System;
using System.Web;
using System.Web.Mvc;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Services.Security;

namespace Nop.Web.Framework
{
    public class PublicStoreAllowNavigationAttribute : ActionFilterAttribute
    {
        private readonly bool _ignore;

        /// <summary>
        /// Ctor 
        /// </summary>
        /// <param name="ignore">Pass false in order to ignore this functionality for a certain action method</param>
        public PublicStoreAllowNavigationAttribute(bool ignore = false)
        {
            this._ignore = ignore;
        }
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            //search the solution by "[PublicStoreAllowNavigation(true)]" keyword 
            //in order to find method available even when a store is closed
            if (_ignore)
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
            
            var permissionService = EngineContext.Current.Resolve<IPermissionService>();
            var publicStoreAllowNavigation = permissionService.Authorize(StandardPermissionProvider.PublicStoreAllowNavigation);
            if (publicStoreAllowNavigation)
                return;

            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}

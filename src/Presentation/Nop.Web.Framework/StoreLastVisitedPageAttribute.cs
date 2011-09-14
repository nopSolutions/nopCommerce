using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;

namespace Nop.Web.Framework
{
    public class StoreLastVisitedPageAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            if (filterContext == null || filterContext.HttpContext == null || filterContext.HttpContext.Request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            //only GET requests
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

            {
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                var pageUrl = webHelper.GetThisPageUrl(true);
                if (!String.IsNullOrEmpty(pageUrl))
                {
                    var workContext = EngineContext.Current.Resolve<IWorkContext>();
                    var customerService = EngineContext.Current.Resolve<ICustomerService>();

                    var previousPageUrl = workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.LastVisitedPage);
                    if (!pageUrl.Equals(previousPageUrl))
                    {
                        customerService.SaveCustomerAttribute(workContext.CurrentCustomer,
                            SystemCustomerAttributeNames.LastVisitedPage, pageUrl);
                    }
                }
            }
        }
    }
}

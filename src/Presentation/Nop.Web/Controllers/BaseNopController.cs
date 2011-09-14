using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Affiliates;
using Nop.Services.Customers;
using Nop.Web.Framework;

namespace Nop.Web.Controllers
{
    [CompressFilter] //comment to disable compression
    public class BaseNopController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            StoreLastVisitedPage(filterContext);

            CheckAffiliate();

            CheckStoreClosed(filterContext);
        }
        
        protected virtual void StoreLastVisitedPage(ActionExecutingContext filterContext)
        {
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            if (filterContext == null || filterContext.HttpContext == null || filterContext.HttpContext.Request == null)
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

        protected virtual void CheckAffiliate()
        {
            if (Request != null &&
                Request.QueryString != null && Request.QueryString["AffiliateId"] != null)
            {
                var affiliateId = Convert.ToInt32(Request.QueryString["AffiliateId"]);

                if (affiliateId > 0)
                {
                    var affiliateService = EngineContext.Current.Resolve<IAffiliateService>();
                    var affiliate = affiliateService.GetAffiliateById(affiliateId);
                    if (affiliate != null && !affiliate.Deleted && affiliate.Active)
                    {
                        var workContext = EngineContext.Current.Resolve<IWorkContext>();
                        if (workContext.CurrentCustomer != null &&
                            workContext.CurrentCustomer.AffiliateId != affiliate.Id)
                        {
                            workContext.CurrentCustomer.AffiliateId = affiliate.Id;
                            var customerService = EngineContext.Current.Resolve<ICustomerService>();
                            customerService.UpdateCustomer(workContext.CurrentCustomer);
                        }
                    }
                }
            }
        }

        protected virtual void CheckStoreClosed(ActionExecutingContext filterContext)
        {
            //don't allow visiting public store except some pages if store is closed
            string actionName = filterContext.ActionDescriptor.ActionName;
            if (String.IsNullOrEmpty(actionName))
                return;

            string controllerName = filterContext.Controller.ToString();
            if (String.IsNullOrEmpty(controllerName))
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;
            
            var storeInformationSettings = EngineContext.Current.Resolve<StoreInformationSettings>();
            if (storeInformationSettings.StoreClosed &&
                !(controllerName.Equals("Nop.Web.Controllers.CustomerController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("Login", StringComparison.InvariantCultureIgnoreCase)) &&
                !(controllerName.Equals("Nop.Web.Controllers.CustomerController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("Logout", StringComparison.InvariantCultureIgnoreCase)))
            {
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                filterContext.Result = new RedirectResult(webHelper.GetStoreLocation() + "StoreClosed.htm");
            }
        }
        
    }
}

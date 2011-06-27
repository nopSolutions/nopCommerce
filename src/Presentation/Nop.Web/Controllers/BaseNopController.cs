using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Affiliates;
using Nop.Services.Customers;

namespace Nop.Web.Controllers
{
    public class BaseNopController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            StoreLastVisitedPage(filterContext);

            CheckAffiliate();
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
    }
}

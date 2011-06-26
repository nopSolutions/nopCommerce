using System;
using System.Web.Mvc;
using Nop.Core;
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

            CheckAffiliate();
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

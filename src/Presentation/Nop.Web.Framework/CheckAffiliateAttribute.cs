using System;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Infrastructure;
using Nop.Services.Affiliates;
using Nop.Services.Customers;

namespace Nop.Web.Framework
{
    public class CheckAffiliateAttribute : ActionFilterAttribute
    {
        private const string AFFILIATE_ID_QUERY_PARAMETER_NAME = "affiliateid";
        private const string AFFILIATE_FRIENDLYURLNAME_QUERY_PARAMETER_NAME = "affiliate";

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

            Affiliate affiliate = null;

            if (request.QueryString != null)
            {
                //try to find by ID ("affiliateId" parameter)
                if (request.QueryString[AFFILIATE_ID_QUERY_PARAMETER_NAME] != null)
                {
                    var affiliateId = Convert.ToInt32(request.QueryString[AFFILIATE_ID_QUERY_PARAMETER_NAME]);
                    if (affiliateId > 0)
                    {
                        var affiliateService = EngineContext.Current.Resolve<IAffiliateService>();
                        affiliate = affiliateService.GetAffiliateById(affiliateId);
                    }
                }
                //try to find by friendly name ("affiliate" parameter)
                else if (request.QueryString[AFFILIATE_FRIENDLYURLNAME_QUERY_PARAMETER_NAME] != null)
                {
                    var friendlyUrlName = request.QueryString[AFFILIATE_FRIENDLYURLNAME_QUERY_PARAMETER_NAME];
                    if (!String.IsNullOrEmpty(friendlyUrlName))
                    {
                        var affiliateService = EngineContext.Current.Resolve<IAffiliateService>();
                        affiliate = affiliateService.GetAffiliateByFriendlyUrlName(friendlyUrlName);
                    }
                }
            }


            if (affiliate != null && !affiliate.Deleted && affiliate.Active)
            {
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                if (workContext.CurrentCustomer.AffiliateId != affiliate.Id)
                {
                    workContext.CurrentCustomer.AffiliateId = affiliate.Id;
                    var customerService = EngineContext.Current.Resolve<ICustomerService>();
                    customerService.UpdateCustomer(workContext.CurrentCustomer);
                }
            }
        }
    }
}

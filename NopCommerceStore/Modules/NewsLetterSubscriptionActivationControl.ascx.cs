using System;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class NewsLetterSubscriptionActivationControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                var NewsLetterSubscriptionGuid = CommonHelper.QueryStringGuid("T");
                bool IsActive = CommonHelper.QueryStringBool("Active");
                if(!NewsLetterSubscriptionGuid.HasValue)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                var subscription = IoCFactory.Resolve<IMessageManager>().GetNewsLetterSubscriptionByGuid(NewsLetterSubscriptionGuid.Value);
                if(subscription == null)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                subscription.Active = IsActive;
               IoCFactory.Resolve<IMessageManager>().UpdateNewsLetterSubscription(subscription);

                if(subscription.Active)
                {
                    lblActivationResult.Text = GetLocaleResourceString("NewsLetterSubscriptionActivation.ResultActivated");
                }
                else
                {
                    lblActivationResult.Text = GetLocaleResourceString("NewsLetterSubscriptionActivation.ResultDectivated");
                }
            }
        }
    }
}
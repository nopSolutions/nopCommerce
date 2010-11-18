using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.Assist;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web
{
    public partial class AssistHostedPaymentReturn : BaseNopFrontendPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //comment this line to process return
            Response.Redirect(CommonHelper.GetStoreLocation());





            if(NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            CommonHelper.SetResponseNoCache(Response);

            if(!Page.IsPostBack)
            {
                Order order = IoC.Resolve<IOrderService>().GetOrderById(CommonHelper.QueryStringInt("Order_IDP"));
                if(order == null || NopContext.Current.User.CustomerId != order.CustomerId)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                if(HostedPaymentSettings.AuthorizeOnly)
                {
                    if(IoC.Resolve<IOrderService>().CanMarkOrderAsAuthorized(order))
                    {
                        IoC.Resolve<IOrderService>().MarkAsAuthorized(order.OrderId);
                    }
                }
                else
                {
                    if(IoC.Resolve<IOrderService>().CanMarkOrderAsPaid(order))
                    {
                        IoC.Resolve<IOrderService>().MarkOrderAsPaid(order.OrderId);
                    }
                }

                Response.Redirect("~/checkoutcompleted.aspx");
            }
        }

        public override bool AllowGuestNavigation
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this page is tracked by 'Online Customers' module
        /// </summary>
        public override bool TrackedByOnlineCustomersModule
        {
            get
            {
                return false;
            }
        }
    }
}

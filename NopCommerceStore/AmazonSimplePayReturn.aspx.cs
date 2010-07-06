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
using NopSolutions.NopCommerce.Payment.Methods.Amazon;

namespace NopSolutions.NopCommerce.Web
{
    public partial class AmazonSimplePayReturn : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            CommonHelper.SetResponseNoCache(Response);

            if(!Page.IsPostBack)
            {
                if(!AmazonHelper.ValidateRequest(Request.QueryString, String.Format("{0}AmazonSimplePayReturn.aspx", CommonHelper.GetStoreLocation()), "GET"))
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                int orderId = Convert.ToInt32(CommonHelper.QueryStringInt("referenceId"));
                Order order = OrderManager.GetOrderById(orderId);
                if(order == null)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }
                if(NopContext.Current.User.CustomerId != order.CustomerId)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                if (SimplePaySettings.SettleImmediately)
                {
                    if (OrderManager.CanMarkOrderAsPaid(order))
                    {
                        OrderManager.MarkOrderAsPaid(order.OrderId);
                    }
                }
                else
                {
                    if (OrderManager.CanMarkOrderAsAuthorized(order))
                    {
                        OrderManager.MarkAsAuthorized(order.OrderId);
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
    }
}

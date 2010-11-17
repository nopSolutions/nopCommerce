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
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using System.Text;

namespace NopSolutions.NopCommerce.Web
{
    public partial class AmazonSimplePayReturn : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            CommonHelper.SetResponseNoCache(Response);

            if (!Page.IsPostBack)
            {
                if (!AmazonHelper.ValidateRequest(Request.QueryString, String.Format("{0}AmazonSimplePayReturn.aspx", CommonHelper.GetStoreLocation()), "GET"))
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                //load order
                int orderId = Convert.ToInt32(CommonHelper.QueryStringInt("referenceId"));
                Order order = IoC.Resolve<IOrderService>().GetOrderById(orderId);
                if (order == null)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }
                //validate order
                if (NopContext.Current.User.CustomerId != order.CustomerId)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                //note
                string recipientEmail = CommonHelper.QueryString("recipientEmail");
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Amazon Simple Pay return page:");
                sb.AppendLine("recipientEmail: " + recipientEmail);
                IoC.Resolve<IOrderService>().InsertOrderNote(order.OrderId, recipientEmail, false, DateTime.UtcNow);

                //paymnent
                if (SimplePaySettings.SettleImmediately)
                {
                    if (IoC.Resolve<IOrderService>().CanMarkOrderAsPaid(order))
                    {
                        IoC.Resolve<IOrderService>().MarkOrderAsPaid(order.OrderId);
                    }
                }
                else
                {
                    if (IoC.Resolve<IOrderService>().CanMarkOrderAsAuthorized(order))
                    {
                        IoC.Resolve<IOrderService>().MarkAsAuthorized(order.OrderId);
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

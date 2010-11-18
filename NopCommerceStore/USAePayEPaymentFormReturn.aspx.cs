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
using NopSolutions.NopCommerce.Payment.Methods.USAePay;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web
{
    public partial class USAePayEPaymentFormReturn : BaseNopFrontendPage
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
                if(EPaymentFormSettings.UsePIN && !EPaymentFormHelper.ValidateResponseSign(Request.Form))
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }
                if(!Request.Form["UMstatus"].Equals("Approved"))
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                int orderId = 0;
                if(!Int32.TryParse(Request.Form["UMinvoice"], out orderId))
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }
                Order order = this.OrderService.GetOrderById(orderId);
                if(order == null || NopContext.Current.User.CustomerId != order.CustomerId)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                string transactionId = Request.Form["UMrefNum"];

                if(EPaymentFormSettings.AuthorizeOnly)
                {
                    //set AuthorizationTransactionID
                    order.AuthorizationTransactionId = transactionId;
                    this.OrderService.UpdateOrder(order);

                    if(this.OrderService.CanMarkOrderAsAuthorized(order))
                    {
                        this.OrderService.MarkAsAuthorized(order.OrderId);
                    }
                }
                else
                {
                    //set CaptureTransactionID
                    order.CaptureTransactionId = transactionId;
                    this.OrderService.UpdateOrder(order);

                    if(this.OrderService.CanMarkOrderAsPaid(order))
                    {
                        this.OrderService.MarkOrderAsPaid(order.OrderId);
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

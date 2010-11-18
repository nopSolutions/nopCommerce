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
using NopSolutions.NopCommerce.Payment.Methods.Moneris;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web
{
    public partial class IdealReturn : BaseNopFrontendPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //comment this line to process return
            Response.Redirect(CommonHelper.GetStoreLocation());

            int failure;
            int orderid;
            if (Request.QueryString["s"] != null && Int32.TryParse(Request.QueryString["s"], out failure) &&
                Request.QueryString["o"] != null && Int32.TryParse(Request.QueryString["o"], out orderid))
            {
                Order o = this.OrderService.GetOrderById(orderid);
                if (o != null)
                {
                    switch (failure)
                    {
                        //Customer cancelled the transaction
                        case 1:
                            //this.plCancel.Visible = true;
                            //this.plError.Visible = false;
                            if (this.OrderService.CanCancelOrder(o))
                            {
                                this.OrderService.CancelOrder(orderid, false);
                            }
                            return;
                        //Ideal error
                        case 2:
                            //this.plCancel.Visible = false;
                            //this.plError.Visible = true;
                            return;
                    }
                }
            }
            Response.Redirect(CommonHelper.GetStoreLocation());
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

using System;
using System.Web.UI;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.Dibs;

namespace NopSolutions.NopCommerce.Web
{
    public partial class DibsFlexWinReturn : BaseNopPage
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
                int orderId = Convert.ToInt32(Request.Form["x"]);
                Order order = OrderManager.GetOrderById(orderId);
                if(order == null)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }
                if(NopContext.Current.User.CustomerId != order.CustomerId)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                string authkey = Request.Form["authkey"];
                int transact = Int32.Parse(Request.Form["transact"]);
                int currency = DibsHelper.GetCurrencyNumberByCode(CurrencyManager.PrimaryStoreCurrency.CurrencyCode);
                int amount = (int)((double)order.OrderTotal * 100);

                if(!authkey.Equals(FlexWinHelper.CalcAuthKey(transact, amount, currency)))
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                if (OrderManager.CanMarkOrderAsPaid(order))
                {
                    OrderManager.MarkOrderAsPaid(order.OrderId);
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

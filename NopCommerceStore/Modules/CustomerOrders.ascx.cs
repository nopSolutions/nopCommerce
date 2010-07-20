//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CustomerOrdersControl : BaseNopUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            if (NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            BindOrders();
            BindRecurringPayments();

            base.OnInit(e);
        }

        private void BindRecurringPayments()
        {
            var recurringPayments = OrderManager.SearchRecurringPayments(NopContext.Current.User.CustomerId,
                0, null);
            if (recurringPayments.Count > 0)
            {
                gvRecurringPayments.DataSource = recurringPayments;
                gvRecurringPayments.DataBind();
            }
            else
            {
                pnlRecurringPayments.Visible = false;
            }
        }

        private void BindOrders()
        {
            var orders = NopContext.Current.User.Orders;
            if (orders.Count > 0)
            {
                rptrOrders.DataSource = orders;
                rptrOrders.DataBind();
            }
        }
        
        protected string GetCycleInfo(RecurringPayment recurringPayment)
        {
            string cycleInfo = string.Empty;
            cycleInfo = string.Format("{0} {1}", recurringPayment.CycleLength, ((RecurringProductCyclePeriodEnum)recurringPayment.CyclePeriod).ToString());
            return cycleInfo;
        }

        protected string GetNextPaymentInfo(RecurringPayment recurringPayment)
        {
            string nextPaymentInfo = string.Empty;
            DateTime? nextPaymentDate = recurringPayment.NextPaymentDate;
            if (nextPaymentDate.HasValue)
            {
                nextPaymentInfo = DateTimeHelper.ConvertToUserTime(nextPaymentDate.Value, DateTimeKind.Utc).ToString();
            }
            return nextPaymentInfo;
        }

        protected string GetInitialOrderInfo(RecurringPayment recurringPayment)
        {
            string initialOrderInfo = string.Empty;
            var initialOrder = recurringPayment.InitialOrder;
            if (initialOrder != null)
            {
                initialOrderInfo = string.Format("<a href=\"{0}orderdetails.aspx?orderid={1}\">{2}</a>", CommonHelper.GetStoreLocation(), initialOrder.OrderId, string.Format(GetLocaleResourceString("Order.RecurringPayments.InitialOrder.View"), initialOrder.OrderId));
            }
            return initialOrderInfo;
        }

        protected string GetOrderTotal(Order order)
        {
            string orderTotalStr = PriceHelper.FormatPrice(order.OrderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false);
            return orderTotalStr;
        }

        protected void btnOrderDetails_Click(object sender, CommandEventArgs e)
        {
            int orderId = Convert.ToInt32(e.CommandArgument);
            Response.Redirect(string.Format("~/orderdetails.aspx?orderid={0}", orderId));
        }

        protected void btnReturnItems_Click(object sender, CommandEventArgs e)
        {
            int orderId = Convert.ToInt32(e.CommandArgument);
            Response.Redirect(string.Format("~/returnitems.aspx?orderid={0}", orderId));
        }
        
        protected void gvRecurringPayments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CancelRecurringPayment")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                var row = gvRecurringPayments.Rows[index];

                var hfRecurringPaymentId = row.FindControl("hfRecurringPaymentId") as HiddenField;

                int recurringPaymentId = int.Parse(hfRecurringPaymentId.Value);
                var rp = OrderManager.GetRecurringPaymentById(recurringPaymentId);
                if (OrderManager.CanCancelRecurringPayment(NopContext.Current.User, rp))
                {
                    rp = OrderManager.CancelRecurringPayment(rp.RecurringPaymentId);
                }
                BindRecurringPayments();
            }
        }

        protected void gvRecurringPayments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var rp = (RecurringPayment)e.Row.DataItem;

                var btnCancelRecurringPayment = e.Row.FindControl("btnCancelRecurringPayment") as Button;
                if (btnCancelRecurringPayment != null)
                {
                    btnCancelRecurringPayment.CommandArgument = e.Row.RowIndex.ToString();
                    btnCancelRecurringPayment.Visible = OrderManager.CanCancelRecurringPayment(NopContext.Current.User, rp);
                }
            }
        }

        protected void rptrOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Order order = (Order)e.Item.DataItem;
                var phReturnRequest = (PlaceHolder)e.Item.FindControl("phReturnRequest");
                if (phReturnRequest != null)
                {
                    phReturnRequest.Visible = OrderManager.IsReturnRequestAllowed(order);
                }
            }
        }
        
    }
}
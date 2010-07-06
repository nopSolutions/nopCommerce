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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.ExportImport;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class PurchasedGiftCardsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillDropDowns();
            }
        }

        protected void FillDropDowns()
        {
            this.ddlOrderStatus.Items.Clear();
            ListItem itemOrderStatus = new ListItem(GetLocaleResourceString("Admin.Common.All"), "0");
            this.ddlOrderStatus.Items.Add(itemOrderStatus);
            var orderStatuses = OrderManager.GetAllOrderStatuses();
            foreach (OrderStatus orderStatus in orderStatuses)
            {
                ListItem item2 = new ListItem(OrderManager.GetOrderStatusName(orderStatus.OrderStatusId), orderStatus.OrderStatusId.ToString());
                this.ddlOrderStatus.Items.Add(item2);
            }
        }

        protected List<GiftCard> GetCards()
        {
            //date filter
            DateTime? startDate = ctrlStartDatePicker.SelectedDate;
            DateTime? endDate = ctrlEndDatePicker.SelectedDate;
            if(startDate.HasValue)
            {
                startDate = DateTimeHelper.ConvertToUtcTime(startDate.Value, DateTimeHelper.CurrentTimeZone);
            }
            if(endDate.HasValue)
            {
                endDate = DateTimeHelper.ConvertToUtcTime(endDate.Value, DateTimeHelper.CurrentTimeZone).AddDays(1);
            }

            //order status filter
            OrderStatusEnum? orderStatus = null;
            int orderStatusId = int.Parse(ddlOrderStatus.SelectedItem.Value);
            if (orderStatusId > 0)
                orderStatus = (OrderStatusEnum)Enum.ToObject(typeof(OrderStatusEnum), orderStatusId);

            //gift card status filter
            bool? isGiftCardActivated = null;
            if (ddlActivated.SelectedValue == "1")
                isGiftCardActivated = true;
            else if (ddlActivated.SelectedValue == "2")
                isGiftCardActivated = false;

            //coupon code filter
            string giftCardCouponCode = txtGiftCardCouponCode.Text;

            var giftCards = OrderManager.GetAllGiftCards(null,
                null, startDate, endDate, orderStatus, null, null, isGiftCardActivated, giftCardCouponCode);
            return giftCards;
        }

        protected void BindGrid()
        {
            var customers = GetCards();
            gvGiftCards.DataSource = customers;
            gvGiftCards.DataBind();
        }

        protected string GetCustomerInfo(GiftCard gc)
        {
            string customerInfo = string.Empty;
            Customer customer = gc.PurchasedOrderProductVariant.Order.Customer;
            if (customer != null)
            {
                if (customer.IsGuest)
                {
                    customerInfo = Server.HtmlEncode(GetLocaleResourceString("Admin.PurchasedGiftCards.Guest"));
                }
                else
                {
                    customerInfo = Server.HtmlEncode(customer.Email);
                }
            }
            return customerInfo;
        }

        protected string GetOrderStatusInfo(GiftCard gc)
        {
            string result = OrderManager.GetOrderStatusName(gc.PurchasedOrderProductVariant.Order.OrderStatusId);
            return result;
        }

        protected string GetInitialValueInfo(GiftCard gc)
        {
            decimal initialValue = GiftCardHelper.GetGiftCardInitialValue(gc);
            string result = PriceHelper.FormatPrice(initialValue, true, false);
            return result;
        }

        protected string GetRemainingAmountInfo(GiftCard gc)
        {
            decimal remainingAmount = GiftCardHelper.GetGiftCardRemainingAmount(gc);
            string result = PriceHelper.FormatPrice(remainingAmount, true, false);
            return result;
        }

        protected string GetPurchasedOnInfo(GiftCard gc)
        {
            string result = DateTimeHelper.ConvertToUserTime(gc.CreatedOn, DateTimeKind.Utc).ToString();
            return result;
        }
                
        protected void gvGiftCards_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvGiftCards.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    BindGrid();
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }
    }
}
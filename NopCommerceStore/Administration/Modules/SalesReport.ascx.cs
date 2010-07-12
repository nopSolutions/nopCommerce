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
using System.Configuration;
using System.Data;
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
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using System.Web.UI.DataVisualization.Charting;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class SalesReportControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                chartOrders.Visible = false;
                FillDropDowns();
            }
        }

        private void FillDropDowns()
        {
            //order statuses
            this.ddlOrderStatus.Items.Clear();
            ListItem itemOrderStatus = new ListItem(GetLocaleResourceString("Admin.Common.All"), "0");
            this.ddlOrderStatus.Items.Add(itemOrderStatus);
            var orderStatuses = OrderManager.GetAllOrderStatuses();
            foreach (OrderStatus orderStatus in orderStatuses)
            {
                ListItem item2 = new ListItem(OrderManager.GetOrderStatusName(orderStatus.OrderStatusId), orderStatus.OrderStatusId.ToString());
                this.ddlOrderStatus.Items.Add(item2);
            }

            //payment statuses
            this.ddlPaymentStatus.Items.Clear();
            ListItem itemPaymentStatus = new ListItem(GetLocaleResourceString("Admin.Common.All"), "0");
            this.ddlPaymentStatus.Items.Add(itemPaymentStatus);
            var paymentStatuses = PaymentStatusManager.GetAllPaymentStatuses();
            foreach (PaymentStatus paymentStatus in paymentStatuses)
            {
                ListItem item2 = new ListItem(PaymentStatusManager.GetPaymentStatusName(paymentStatus.PaymentStatusId), paymentStatus.PaymentStatusId.ToString());
                this.ddlPaymentStatus.Items.Add(item2);
            }

            //countries
            ddlBillingCountry.Items.Clear();
            ListItem itemBillingCountry = new ListItem(GetLocaleResourceString("Admin.Common.All"), "0");
            this.ddlBillingCountry.Items.Add(itemBillingCountry);
            var countries = CountryManager.GetAllCountriesForBilling();
            foreach (var country in countries)
            {
                ListItem ddlCountryItem2 = new ListItem(country.Name, country.CountryId.ToString());
                ddlBillingCountry.Items.Add(ddlCountryItem2);
            }
        }

        protected void BindGrid()
        {
            DateTime? startDate = ctrlStartDatePicker.SelectedDate;
            DateTime? endDate = ctrlEndDatePicker.SelectedDate;
            if (startDate.HasValue)
            {
                startDate = DateTimeHelper.ConvertToUtcTime(startDate.Value, DateTimeHelper.CurrentTimeZone);
            }
            if (endDate.HasValue)
            {
                endDate = DateTimeHelper.ConvertToUtcTime(endDate.Value, DateTimeHelper.CurrentTimeZone).AddDays(1);
            }

            OrderStatusEnum? orderStatus = null;
            int orderStatusId = int.Parse(ddlOrderStatus.SelectedItem.Value);
            if (orderStatusId > 0)
                orderStatus = (OrderStatusEnum)Enum.ToObject(typeof(OrderStatusEnum), orderStatusId);

            PaymentStatusEnum? paymentStatus = null;
            int paymentStatusId = int.Parse(ddlPaymentStatus.SelectedItem.Value);
            if (paymentStatusId > 0)
                paymentStatus = (PaymentStatusEnum)Enum.ToObject(typeof(PaymentStatusEnum), paymentStatusId);
            int billingCountryID = int.Parse(ddlBillingCountry.SelectedItem.Value);

            var report = OrderManager.OrderProductVariantReport(startDate, endDate, orderStatus, paymentStatus, billingCountryID);
            if (report.Count == 0)
            {
                chartOrders.Visible = false;
            }
            else
            {
                chartOrders.Series[0].Points.Clear();
                chartOrders.Visible = true;
                foreach (OrderProductVariantReportLine repLine in report)
                {
                    var dp = new DataPoint();
                    dp.YValues = new double[1] { (double)repLine.PriceExclTax };
                    dp.LegendText = GetProductVariantName(repLine.ProductVariantId);
                    dp.ToolTip = dp.LegendText;
                    chartOrders.Series[0].Points.Add(dp);
                }
                chartOrders.DataBind();
            }

            gvOrders.DataSource = report;
            gvOrders.DataBind();
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

        public string GetProductVariantUrl(int productVariantId)
        {
            string result = string.Empty;
            ProductVariant productVariant = ProductManager.GetProductVariantById(productVariantId);
            if (productVariant != null)
                result = "ProductVariantDetails.aspx?ProductVariantID=" + productVariant.ProductVariantId.ToString();
            else
                result = "Not available. Product variant ID=" + productVariantId.ToString();
            return result;
        }

        public string GetProductVariantName(int productVariantId)
        {
            ProductVariant productVariant = ProductManager.GetProductVariantById(productVariantId);
            if (productVariant != null)
                return productVariant.FullProductName;
            return "Not available. ID=" + productVariantId.ToString();
        }
    }
}
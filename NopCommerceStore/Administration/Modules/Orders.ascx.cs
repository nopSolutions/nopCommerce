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
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.ExportImport;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class OrdersControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillDropDowns();
                SetDefaultValues();

                //buttons
                btnPrintPdfPackagingSlips.Visible = SettingManager.GetSettingValueBoolean("Features.SupportPDF");
                btnExportXLS.Visible = SettingManager.GetSettingValueBoolean("Features.SupportExcel");
            }
        }

        protected void SetDefaultValues()
        {
        }

        protected List<Order> GetOrders()
        {
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

            OrderStatusEnum? orderStatus = null;
            int orderStatusId = int.Parse(ddlOrderStatus.SelectedItem.Value);
            if (orderStatusId > 0)
                orderStatus = (OrderStatusEnum)Enum.ToObject(typeof(OrderStatusEnum), orderStatusId);

            PaymentStatusEnum? paymentStatus = null;
            int paymentStatusId = int.Parse(ddlPaymentStatus.SelectedItem.Value);
            if (paymentStatusId > 0)
                paymentStatus = (PaymentStatusEnum)Enum.ToObject(typeof(PaymentStatusEnum), paymentStatusId);

            ShippingStatusEnum? shippingStatus = null;
            int shippingStatusId = int.Parse(ddlShippingStatus.SelectedItem.Value);
            if (shippingStatusId > 0)
                shippingStatus = (ShippingStatusEnum)Enum.ToObject(typeof(ShippingStatusEnum), shippingStatusId);

            string orderGuid = txtOrderGuid.Text.Trim();

            var orders = OrderManager.SearchOrders(startDate, endDate,
                txtCustomerEmail.Text, orderStatus, paymentStatus, shippingStatus, orderGuid);
            return orders;
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

            this.ddlPaymentStatus.Items.Clear();
            ListItem itemPaymentStatus = new ListItem(GetLocaleResourceString("Admin.Common.All"), "0");
            this.ddlPaymentStatus.Items.Add(itemPaymentStatus);
            var paymentStatuses = PaymentStatusManager.GetAllPaymentStatuses();
            foreach (PaymentStatus paymentStatus in paymentStatuses)
            {
                ListItem item2 = new ListItem(PaymentStatusManager.GetPaymentStatusName(paymentStatus.PaymentStatusId), paymentStatus.PaymentStatusId.ToString());
                this.ddlPaymentStatus.Items.Add(item2);
            }

            this.ddlShippingStatus.Items.Clear();
            ListItem itemShippingStatus = new ListItem(GetLocaleResourceString("Admin.Common.All"), "0");
            this.ddlShippingStatus.Items.Add(itemShippingStatus);
            var shippingStatuses = ShippingStatusManager.GetAllShippingStatuses();
            foreach (ShippingStatus shippingStatus in shippingStatuses)
            {
                ListItem item2 = new ListItem(ShippingStatusManager.GetShippingStatusName(shippingStatus.ShippingStatusId), shippingStatus.ShippingStatusId.ToString());
                this.ddlShippingStatus.Items.Add(item2);
            }
        }

        protected void btnExportXML_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    string fileName = string.Format("orders_{0}.xml", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

                    var orders = GetOrders();
                    string xml = ExportManager.ExportOrdersToXml(orders);
                    CommonHelper.WriteResponseXml(xml, fileName);
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void btnExportXLS_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    string fileName = string.Format("orders_{0}_{1}.xls", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));
                    string filePath = string.Format("{0}files\\ExportImport\\{1}", HttpContext.Current.Request.PhysicalApplicationPath, fileName);
                    var orders = GetOrders();

                    ExportManager.ExportOrdersToXls(filePath, orders);
                    CommonHelper.WriteResponseXls(filePath, fileName);
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void BtnPrintPdfPackagingSlips_OnClick(object sender, EventArgs e)
        {
            try
            {
                string fileName = String.Format("packagingslips_{0}_{1}.pdf", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));
                string filePath = String.Format("{0}files\\exportimport\\{1}", HttpContext.Current.Request.PhysicalApplicationPath, fileName);

                PDFHelper.PrintPackagingSlipsToPdf(GetOrders(), filePath);

                CommonHelper.WriteResponsePdf(filePath, fileName);
            }
            catch(Exception ex)
            {
                ProcessException(ex);
            }
        }

        protected void BindGrid()
        {
            var orders = GetOrders();
            if (orders.Count > 0)
            {
                this.gvOrders.Visible = true;
                this.lblNoOrdersFound.Visible = false;
                this.gvOrders.DataSource = orders;
                this.gvOrders.DataBind();
            }
            else
            {
                this.gvOrders.Visible = false;
                this.lblNoOrdersFound.Visible = true;
            }
        }

        protected string GetCustomerInfo(int customerId)
        {
            string customerInfo = string.Empty;
            Customer customer = CustomerManager.GetCustomerById(customerId);
            if (customer != null)
            {
                if (customer.IsGuest)
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, GetLocaleResourceString("Admin.Orders.CustomerColumn.Guest"));
                }
                else
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, Server.HtmlEncode(customer.Email));
                }
            }
            return customerInfo;
        }

        protected void gvOrders_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvOrders.PageIndex = e.NewPageIndex;
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

        protected void btnGoDirectlyToOrderNumber_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    int orderId = 0;
                    if (int.TryParse(txtOrderId.Text.Trim(), out orderId))
                    {
                        string url = string.Format("{0}OrderDetails.aspx?OrderID={1}", CommonHelper.GetStoreAdminLocation(), orderId);
                        Response.Redirect(url);
                    }
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }
    }
}
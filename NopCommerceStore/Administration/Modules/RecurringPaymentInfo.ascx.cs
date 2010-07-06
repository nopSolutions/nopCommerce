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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class RecurringPaymentInfoControl : BaseNopAdministrationUserControl
    {
        private void FillDropDowns()
        {
            CommonHelper.FillDropDownWithEnum(this.ddlCyclePeriod, typeof(RecurringProductCyclePeriodEnum));
        }

        private void BindData()
        {
            RecurringPayment recurringPayment = OrderManager.GetRecurringPaymentById(this.RecurringPaymentId);
            if (recurringPayment != null)
            {
                Order initialOrder = recurringPayment.InitialOrder;
                if (initialOrder != null)
                {
                    this.lblInitialOrder.Text = string.Format("<a href=\"OrderDetails.aspx?OrderID={0}\">{1}</a>", initialOrder.OrderId, GetLocaleResourceString("Admin.RecurringPaymentInfo.InitialOrder.View"));
                }
                else
                {
                    this.lblInitialOrder.Text = "Not available";
                }
                Customer customer = recurringPayment.Customer;
                if (customer != null)
                {
                    this.lblCustomer.Text = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, GetLocaleResourceString("Admin.RecurringPaymentInfo.Customer.View"));
                }
                else
                {
                    this.lblCustomer.Text = "Not available";
                }
                

                this.txtCycleLength.Value = recurringPayment.CycleLength;
                CommonHelper.SelectListItem(this.ddlCyclePeriod, recurringPayment.CyclePeriod);
                this.txtTotalCycles.Value = recurringPayment.TotalCycles;
                this.lblCyclesRemaining.Text = recurringPayment.CyclesRemaining.ToString();
                this.lblRecurringPaymentType.Text = CommonHelper.ConvertEnum(recurringPayment.RecurringPaymentType.ToString());
                this.lblStartDate.Text = DateTimeHelper.ConvertToUserTime(recurringPayment.StartDate, DateTimeKind.Utc).ToString();
                this.cbIsActive.Checked = recurringPayment.IsActive;
            }
            else
            {
                Response.Redirect("~/RecurringPayments.aspx");
            }
        }

        private void BindHistory()
        {
            RecurringPayment recurringPayment = OrderManager.GetRecurringPaymentById(this.RecurringPaymentId);
            if (recurringPayment != null)
            {
                DateTime? nextPaymentDate = recurringPayment.NextPaymentDate;
                if (nextPaymentDate.HasValue)
                {
                    lblNextPaymentDate.Text = string.Format(GetLocaleResourceString("Admin.RecurringPaymentInfo.NextPaymentDateIs"), DateTimeHelper.ConvertToUserTime(nextPaymentDate.Value, DateTimeKind.Utc).ToString());
                    lblNextPaymentDate.Visible = true;
                    btnProcessNextPayment.Visible = true;
                }
                else
                {
                    lblNextPaymentDate.Visible = false;
                    btnProcessNextPayment.Visible = false;
                }

                btnCancelPayment.Visible = OrderManager.CanCancelRecurringPayment(NopContext.Current.User, recurringPayment);

                var recurringPaymentHistoryCollection = OrderManager.SearchRecurringPaymentHistory(recurringPayment.RecurringPaymentId, 0);
                gvRecurringPaymentHistory.DataSource = recurringPaymentHistoryCollection;
                gvRecurringPaymentHistory.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.FillDropDowns();
                this.BindData();
                this.BindHistory();
            }
        }

        protected void btnProcessNextPayment_Click(object sender, EventArgs e)
        {
            try
            {
                OrderManager.ProcessNextRecurringPayment(this.RecurringPaymentId);
                this.BindData();
                this.BindHistory();
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected void btnCancelPayment_Click(object sender, EventArgs e)
        {
            try
            {
                RecurringPayment rp = OrderManager.GetRecurringPaymentById(this.RecurringPaymentId);
                if (OrderManager.CanCancelRecurringPayment(NopContext.Current.User, rp))
                {
                    rp = OrderManager.CancelRecurringPayment(rp.RecurringPaymentId);
                }
                this.BindHistory();
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }
        
        public RecurringPayment SaveInfo()
        {
            int cycleLength = txtCycleLength.Value;
            RecurringProductCyclePeriodEnum cyclePeriod = (RecurringProductCyclePeriodEnum)Enum.ToObject(typeof(RecurringProductCyclePeriodEnum), int.Parse(this.ddlCyclePeriod.SelectedItem.Value));
            int totalCycles = txtTotalCycles.Value;
            bool isActive = cbIsActive.Checked;

            RecurringPayment recurringPayment = OrderManager.GetRecurringPaymentById(this.RecurringPaymentId);
            if (recurringPayment != null)
            {
                recurringPayment = OrderManager.UpdateRecurringPayment(recurringPayment.RecurringPaymentId,
                    recurringPayment.InitialOrderId,
                    cycleLength,
                    (int)cyclePeriod,
                    totalCycles,
                    recurringPayment.StartDate,
                    isActive,
                    recurringPayment.Deleted,
                    recurringPayment.CreatedOn);
            }
            else
            {
                Response.Redirect("~/RecurringPayments.aspx");
            }

            return recurringPayment;
        }

        public int RecurringPaymentId
        {
            get
            {
                return CommonHelper.QueryStringInt("RecurringPaymentId");
            }
        }
    }
}
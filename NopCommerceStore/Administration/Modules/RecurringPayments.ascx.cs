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
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class RecurringPaymentsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindGrid();
            }
        }

        protected void gvRecurringPayments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvRecurringPayments.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void BindGrid()
        {
            var recurringPayments = OrderManager.SearchRecurringPayments(0, 0, null);
            if (recurringPayments.Count > 0)
            {
                this.gvRecurringPayments.Visible = true;
                this.lblNoRecurringPayments.Visible = false;
                this.gvRecurringPayments.DataSource = recurringPayments;
                this.gvRecurringPayments.DataBind();
            }
            else
            {
                this.gvRecurringPayments.Visible = false;
                this.lblNoRecurringPayments.Visible = true;
            }
        }

        protected string GetCustomerInfo(RecurringPayment recurringPayment)
        {
            string customerInfo = string.Empty;
            Customer customer = recurringPayment.Customer;
            if (customer != null)
            {
                if (customer.IsGuest)
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, GetLocaleResourceString("Admin.RecurringPayments.CustomerColumn.Guest"));
                }
                else
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, Server.HtmlEncode(customer.FullName));
                }
            }
            return customerInfo;
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
    }
}
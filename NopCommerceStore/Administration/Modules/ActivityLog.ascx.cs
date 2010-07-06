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
using NopSolutions.NopCommerce.Common.Utils;
using System.IO;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using System.Collections.Generic;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ActivityLogControl : BaseNopAdministrationUserControl
    {
        protected class RowHelper
        {
            private ActivityLog _activityLog;
            private ActivityLogType _activityLogType;
            private Customer _customer;

            public RowHelper(ActivityLog activityLog)
            {
                _activityLog = activityLog;
                _activityLogType = _activityLog.ActivityLogType;
                _customer = _activityLog.Customer;
            }

            public int ActivityLogID
            {
                get { return _activityLog.ActivityLogId; }
            }

            public string ActivityLogType
            {
                get { return _activityLogType.Name; }
            }

            public Customer Customer
            {
                get
                {
                    return _customer;
                }
            }

            public string Message
            {
                get { return _activityLog.Comment; }
            }

            public DateTime CreateOn
            {
                get { return DateTimeHelper.ConvertToUserTime(_activityLog.CreatedOn, DateTimeKind.Utc); }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            } 
        }

        protected void SetDefaultValues()
        {
            int days = CommonHelper.QueryStringInt("ShowDays");
            if (days > 0)
            {
                ctrlCreatedOnFromDatePicker.SelectedDate = DateTime.UtcNow.AddDays(-days);
            }
        }

        protected void FillActivityLogTypesDropDowns()
        {
            ddlActivityLogType.Items.Clear();

            var allItem = new ListItem(GetLocaleResourceString("Admin.ActivityLog.AllActivityLogType"), "0");
            ddlActivityLogType.Items.Add(allItem);

            var activityLogTypes = CustomerActivityManager.GetAllActivityTypes();
            foreach (var activityType in activityLogTypes)
            {
                var item = new ListItem(activityType.Name, activityType.ActivityLogTypeId.ToString());
                ddlActivityLogType.Items.Add(item);
            }

            CommonHelper.SelectListItem(this.ddlActivityLogType, 0);
        }

        protected List<RowHelper> GetActivityLogs()
        {
            DateTime? startDate = ctrlCreatedOnFromDatePicker.SelectedDate;
            DateTime? endDate = ctrlCreatedOnToDatePicker.SelectedDate;
            if (startDate.HasValue)
            {
                startDate = DateTimeHelper.ConvertToUtcTime(startDate.Value, DateTimeHelper.CurrentTimeZone);
            }
            if (endDate.HasValue)
            {
                endDate = DateTimeHelper.ConvertToUtcTime(endDate.Value, DateTimeHelper.CurrentTimeZone).AddDays(1);
            }

            string customerEmail = txtCustomerEmail.Text.Trim();
            string customerName = txtCustomerName.Text.Trim();

            int activityLogTypeId = int.Parse(this.ddlActivityLogType.SelectedItem.Value);
            int totalRecords = 0;
            var activityLogs = CustomerActivityManager.GetAllActivities(
                startDate, endDate, customerEmail, customerName, activityLogTypeId,
                int.MaxValue, 0, out totalRecords);

            var result = new List<RowHelper>();
            foreach (var activityLog in activityLogs)
                result.Add(new RowHelper(activityLog));
            
            return result;
        }

        protected string GetCustomerInfo(Customer customer)
        {
            if (customer != null)
                return string.Format(
                    "<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>",
                    customer.CustomerId,
                    Server.HtmlEncode(customer.Email));

            return string.Empty;
        }

        protected void BindGrid()
        {
            var activityLogs = GetActivityLogs();
            gvActivityLog.DataSource = activityLogs;
            gvActivityLog.DataBind();
        }

        private void BindData()
        {
            FillActivityLogTypesDropDowns();
            SetDefaultValues();
            phCustomerName.Visible = CustomerManager.UsernamesEnabled;
        }

        protected void gvActivityLog_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvActivityLog.PageIndex = e.NewPageIndex;
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

        protected void DeleteActivityLogButton_OnCommand(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "DeleteActivityLog")
            {
                CustomerActivityManager.DeleteActivity(Convert.ToInt32(e.CommandArgument));
                BindGrid();
            }
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            try
            {
                CustomerActivityManager.ClearAllActivities();
                BindGrid();
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }
    }
}
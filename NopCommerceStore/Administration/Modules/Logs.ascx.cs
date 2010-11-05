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
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.IoC;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class LogsControl : BaseNopAdministrationUserControl
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
            ddlLogType.Items.Clear();

            var allItem = new ListItem(GetLocaleResourceString("Admin.Log.AllLogTypes"), "0");
            ddlLogType.Items.Add(allItem);

            CommonHelper.FillDropDownWithEnum(this.ddlLogType, typeof(LogTypeEnum), false);
        }

        protected PagedList<Log> GetLogItems()
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

            string message = txtMessage.Text.Trim();

            int logTypeId = int.Parse(this.ddlLogType.SelectedItem.Value);
            var result = IoCFactory.Resolve<ILogService>().GetAllLogs(startDate, endDate,
                message, logTypeId, 0, int.MaxValue);

            return result;
        }


        protected void BindGrid()
        {
            var log = GetLogItems();
            gvLogs.DataSource = log;
            gvLogs.DataBind();
            //btnClear.Visible = log.TotalCount > 0;
        }

        protected void gvLogs_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLogs.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
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

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                IoCFactory.Resolve<ILogService>().ClearLog();
                BindGrid();
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected string GetCustomerInfo(int customerId)
        {
            string customerInfo = string.Empty;
            Customer customer = IoCFactory.Resolve<ICustomerService>().GetCustomerById(customerId);
            if (customer != null)
            {
                if (customer.IsGuest)
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, GetLocaleResourceString("Admin.Logs.Customer.Guest"));
                }
                else
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, Server.HtmlEncode(customer.Email));
                }
            }
            return customerInfo;
        }
        
        protected void DeleteLogButton_OnCommand(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "DeleteLog")
            {
                IoCFactory.Resolve<ILogService>().DeleteLog(Convert.ToInt32(e.CommandArgument));
                BindGrid();
            }
        }
    }
}
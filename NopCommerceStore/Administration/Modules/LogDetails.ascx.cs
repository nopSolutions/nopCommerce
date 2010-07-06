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
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class LogDetailsControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Log log = LogManager.GetLogById(this.LogId);
            if (log != null)
            {
                this.lblLogType.Text = Server.HtmlEncode(log.LogType.ToString());
                this.lblSeverity.Text = log.Severity.ToString();
                this.lblMessage.Text = Server.HtmlEncode(log.Message);
                this.lblException.Text = Server.HtmlEncode(log.Exception);
                this.lblIPAddress.Text = Server.HtmlEncode(log.IPAddress);
                Customer customer = log.Customer;
                if (customer != null)
                    this.lblCustomer.Text = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, Server.HtmlEncode(customer.Email));
                lblPageURL.Text = Server.HtmlEncode(log.PageUrl);
                lblReferrerURL.Text = Server.HtmlEncode(log.ReferrerUrl);
                this.lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(log.CreatedOn, DateTimeKind.Utc).ToString();
            }
            else
                Response.Redirect("Logs.aspx");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                LogManager.DeleteLog(this.LogId);
                Response.Redirect("Logs.aspx");
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        public int LogId
        {
            get
            {
                return CommonHelper.QueryStringInt("LogId");
            }
        }
    }
}
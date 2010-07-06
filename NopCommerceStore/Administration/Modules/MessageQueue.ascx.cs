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
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class MessageQueueControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SetDefaultValues();
            }
        }

        protected void SetDefaultValues()
        {
        }

        protected void BindGrid()
        {
            DateTime? startDate = ctrlStartDatePicker.SelectedDate;
            DateTime? endDate = ctrlEndDatePicker.SelectedDate;
            DateTime startDateTmp = DateTime.UtcNow;
            if(startDate.HasValue)
            {
                startDate = DateTimeHelper.ConvertToUtcTime(startDate.Value, DateTimeHelper.CurrentTimeZone);
            }
            if(endDate.HasValue)
            {
                endDate = DateTimeHelper.ConvertToUtcTime(endDate.Value, DateTimeHelper.CurrentTimeZone).AddDays(1);
            }

            string fromEmail = txtFromEmail.Text;
            string toEmail = txtToEmail.Text;
            bool loadNotSentItemsOnly = cbLoadNotSentItemsOnly.Checked;
            int maxSendTries = txtMaxSendTries.Value;

            var queuedEmails = MessageManager.GetAllQueuedEmails(fromEmail, toEmail,
                startDate, endDate, 0, loadNotSentItemsOnly, maxSendTries);
            if (queuedEmails.Count > 0)
            {
                gvQueuedEmails.Visible = true;
                lblQueuedEmailsFound.Visible = false;
                gvQueuedEmails.DataSource = queuedEmails;
                gvQueuedEmails.DataBind();
            }
            else
            {
                gvQueuedEmails.Visible = false;
                lblQueuedEmailsFound.Visible = true;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();

            base.OnPreRender(e);
        }

        protected void gvQueuedEmails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvQueuedEmails.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void LoadButton_Click(object sender, EventArgs e)
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

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (GridViewRow row in gvQueuedEmails.Rows)
                {
                    var cbQueuedEmail = row.FindControl("cbQueuedEmail") as CheckBox;
                    var hfQueuedEmailId = row.FindControl("hfQueuedEmailId") as HiddenField;

                    bool isChecked = cbQueuedEmail.Checked;
                    int queuedEmailId = int.Parse(hfQueuedEmailId.Value);
                    if (isChecked)
                    {
                        MessageManager.DeleteQueuedEmail(queuedEmailId);
                    }
                }

                BindGrid();
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }

        protected string GetFromInfo(QueuedEmail queuedEmail)
        {
            string result = string.Empty;
            if (String.IsNullOrEmpty(queuedEmail.FromName))
                result = queuedEmail.From;
            else
                result = string.Format("{0} ({1})", queuedEmail.FromName, queuedEmail.From);

            return Server.HtmlEncode(result);
        }

        protected string GetToInfo(QueuedEmail queuedEmail)
        {
            string result = string.Empty;
            if (String.IsNullOrEmpty(queuedEmail.ToName))
                result = queuedEmail.To;
            else
                result = string.Format("{0} ({1})", queuedEmail.ToName, queuedEmail.To);

            return Server.HtmlEncode(result);
        }

        protected string GetSentOnInfo(QueuedEmail queuedEmail)
        {
            if (!queuedEmail.SentOn.HasValue)
                return "Not sent yet";
            else
                return string.Format("Sent on {0}", DateTimeHelper.ConvertToUserTime(queuedEmail.SentOn.Value, DateTimeKind.Utc));
        }

        protected void btnGoDirectlyToEmailNumber_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    int emailId = 0;
                    if (int.TryParse(txtEmailId.Text.Trim(), out emailId))
                    {
                        string url = string.Format("{0}MessageQueueDetails.aspx?QueuedEmailID={1}", CommonHelper.GetStoreAdminLocation(), emailId);
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
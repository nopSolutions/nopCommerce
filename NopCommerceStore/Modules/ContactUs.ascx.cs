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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.Common.Utils;
 

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ContactUsControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                BindData();
        }

        protected void BindData()
        {
            if (NopContext.Current.User != null && !NopContext.Current.User.IsGuest)
            {
                txtFullName.Text = NopContext.Current.User.FullName;
                txtEmail.Text = NopContext.Current.User.Email;
            }
        }

        protected void btnContactUs_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (String.IsNullOrEmpty(txtEnquiry.Text))
                        return;
                    string email = txtEmail.Text.Trim();
                    string fullName = txtFullName.Text.Trim();
                    string subject = string.Format("{0}. {1}", SettingManager.StoreName, "Contact us");
                    string body = MessageManager.FormatContactUsFormText(txtEnquiry.Text);

                    var from = new MailAddress(email, fullName);
                    
                    var emailAccount = MessageManager.DefaultEmailAccount;
                    //required for some SMTP servers
                    if (SettingManager.GetSettingValueBoolean("Email.UseSystemEmailForContactUsForm"))
                    {
                        from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
                        body = string.Format("<b>From</b>: {0} - {1}<br /><br />{2}", Server.HtmlEncode(fullName), Server.HtmlEncode(email), body);
                    }
                    var to = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
                    MessageManager.InsertQueuedEmail(5, from, to, string.Empty, string.Empty, subject, body,
                        DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);

                    pnlResult.Visible = true;
                    pnlContactUs.Visible = false;
                }
                catch (Exception exc)
                {
                    LogManager.InsertLog(LogTypeEnum.MailError, string.Format("Error sending \"Contact us\" email."), exc);
                }
            }
        }
    }
}
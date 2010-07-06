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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class PrivateMessagesSendControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
            LoadBBCodeEditorJS();
        }

        private void LoadBBCodeEditorJS()
        {
            string bbCodeJS = "<script src='" + Page.ResolveUrl("~/editors/BBEditor/ed.js") + "' type='text/javascript'></script>";
            Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "BBCodeEditor", bbCodeJS, false);
        }

        private void BindData()
        {
            Customer toCustomer = null;
            var replyToPM = ForumManager.GetPrivateMessageById(this.ReplyToMessageId);
            if (replyToPM != null)
            {
                if (replyToPM.ToUserId == NopContext.Current.User.CustomerId || replyToPM.FromUserId == NopContext.Current.User.CustomerId)
                {
                    toCustomer = replyToPM.FromUser;
                    txtSubject.Text = string.Format("Re: {0}", replyToPM.Subject);
                }
                else
                {
                    Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
                }
            }
            else
            {
                toCustomer = CustomerManager.GetCustomerById(this.ToCustomerId);
            }

            if (toCustomer == null || toCustomer.IsGuest)
            {
                Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
            }

            lblSendTo.Text = Server.HtmlEncode(CustomerManager.FormatUserName(toCustomer));
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string subject = txtSubject.Text.Trim();
                string message = txtMessageBBCode.Text.Trim();

                if (String.IsNullOrEmpty(subject))
                    throw new NopException(GetLocaleResourceString("PrivateMessages.SubjectCannotBeEmpty"));

                if (String.IsNullOrEmpty(message))
                    throw new NopException(GetLocaleResourceString("PrivateMessages.MessageCannotBeEmpty"));

                Customer toCustomer = null;
                var replyToPM = ForumManager.GetPrivateMessageById(this.ReplyToMessageId);
                if (replyToPM != null)
                {
                    if (replyToPM.ToUserId == NopContext.Current.User.CustomerId || replyToPM.FromUserId == NopContext.Current.User.CustomerId)
                    {
                        toCustomer = replyToPM.FromUser;
                    }
                    else
                    {
                        Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
                    }
                }
                else
                {
                    toCustomer = CustomerManager.GetCustomerById(this.ToCustomerId);
                }

                if (toCustomer == null || toCustomer.IsGuest)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
                }

                var pm = ForumManager.InsertPrivateMessage(NopContext.Current.User.CustomerId, toCustomer.CustomerId,
                    subject, message, false, false, false, DateTime.UtcNow);

                Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx?tab=sent");
            }
            catch (Exception exc)
            {
                pnlError.Visible = true;
                lErrorMessage.Text = Server.HtmlEncode(exc.Message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
        }

        public int ToCustomerId
        {
            get
            {
                return CommonHelper.QueryStringInt("ToId");
            }
        }

        public int ReplyToMessageId
        {
            get
            {
                return CommonHelper.QueryStringInt("R");
            }
        }
    }
}

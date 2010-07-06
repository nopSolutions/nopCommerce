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

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class PrivateMessagesViewControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            var pm = ForumManager.GetPrivateMessageById(this.PrivateMessageId);
            if (pm != null)
            {
                if (pm.ToUserId != NopContext.Current.User.CustomerId && pm.FromUserId != NopContext.Current.User.CustomerId)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
                }

                if (!pm.IsRead && pm.ToUserId == NopContext.Current.User.CustomerId)
                {
                    pm = ForumManager.UpdatePrivateMessage(pm.PrivateMessageId, pm.FromUserId, pm.ToUserId,
                        pm.Subject, pm.Text, true, pm.IsDeletedByAuthor, pm.IsDeletedByRecipient, pm.CreatedOn);
                }
            }
            else
            {
                Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
            }

            lblFrom.Text = Server.HtmlEncode(CustomerManager.FormatUserName(pm.FromUser));
            lblTo.Text = Server.HtmlEncode(CustomerManager.FormatUserName(pm.ToUser));
            lblSubject.Text = Server.HtmlEncode(pm.Subject);
            lblMessage.Text = ForumManager.FormatPrivateMessageText(pm.Text);
        }

        protected void btnReply_Click(object sender, EventArgs e)
        {
            var pm = ForumManager.GetPrivateMessageById(this.PrivateMessageId);
            if (pm != null)
            {
                string replyURL = string.Format("{0}sendpm.aspx?r={1}", CommonHelper.GetStoreLocation(), pm.PrivateMessageId);
                Response.Redirect(replyURL);
            }
            else
            {
                Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var pm = ForumManager.GetPrivateMessageById(this.PrivateMessageId);
            if (pm != null)
            {
                if (pm.FromUserId == NopContext.Current.User.CustomerId)
                {
                    pm = ForumManager.UpdatePrivateMessage(pm.PrivateMessageId, pm.FromUserId, pm.ToUserId,
                         pm.Subject, pm.Text, pm.IsRead, true, pm.IsDeletedByRecipient, pm.CreatedOn);
                }

                if (pm != null)
                {
                    if (pm.ToUserId == NopContext.Current.User.CustomerId)
                    {
                        pm = ForumManager.UpdatePrivateMessage(pm.PrivateMessageId, pm.FromUserId, pm.ToUserId,
                             pm.Subject, pm.Text, pm.IsRead, pm.IsDeletedByAuthor, true, pm.CreatedOn);
                    }
                }
            }
            Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
        }

        public int PrivateMessageId
        {
            get
            {
                return CommonHelper.QueryStringInt("PM");
            }
        }
    }
}

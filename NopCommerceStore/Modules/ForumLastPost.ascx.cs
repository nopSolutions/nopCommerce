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
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ForumLastPostControl : BaseNopUserControl
    {
        ForumPost forumPost = null;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public override void DataBind()
        {
            base.DataBind();
            this.BindData();
        }

        public void BindData()
        {
            if (forumPost != null)
            {
                lblLastPostDate.Text = DateTimeHelper.ConvertToUserTime(forumPost.CreatedOn, DateTimeKind.Utc).ToString("f");
                var forumTopic = forumPost.Topic;
                if (forumTopic != null)
                {
                    hlTopic.Text = Server.HtmlEncode(ForumManager.StripTopicSubject(forumTopic.Subject));
                    hlTopic.ToolTip = Server.HtmlEncode(forumTopic.Subject);
                    hlTopic.NavigateUrl = SEOHelper.GetForumTopicUrl(forumTopic);
                }
                var customer = forumPost.User;
                if(customer != null)
                {
                    if(CustomerManager.AllowViewingProfiles && !customer.IsGuest)
                    {
                        hlUser.Text = Server.HtmlEncode(CustomerManager.FormatUserName(customer, true));
                        hlUser.NavigateUrl = SEOHelper.GetUserProfileUrl(customer.CustomerId);
                        lblUser.Visible = false;
                    }
                    else
                    {
                        lblUser.Text = Server.HtmlEncode(CustomerManager.FormatUserName(customer, true));
                        hlUser.Visible = false;
                    }
                }

                pnlPostInfo.Visible = true;
                pnlNoPost.Visible = false;
            }
            else
            {
                pnlPostInfo.Visible = false;
                pnlNoPost.Visible = true;
            }
        }

        public ForumPost ForumPost
        {
            get
            {
                return forumPost;
            }
            set
            {
                forumPost = value;
            }
        }

        [DefaultValue(false)]
        public bool ShowTopic
        {
            get
            {
                object obj2 = this.ViewState["ShowTopic"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["ShowTopic"] = value;
            }
        }
    }
}
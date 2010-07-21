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
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ForumActiveDiscussionsControl : BaseNopUserControl
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
            if (!ForumManager.ForumsEnabled)
            {
                this.Visible = false;
                return;
            }
            var forumTopics = ForumManager.GetActiveTopics(this.ForumId, this.TopicCount);
            if (forumTopics.Count > 0)
            {
                rptrTopics.DataSource = forumTopics;
                rptrTopics.DataBind();
            }
            else
            {
                this.Visible = false;
            }
        }
        
        protected void rptrTopics_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var forumTopic = e.Item.DataItem as ForumTopic;
                var customer = forumTopic.User;

                var hlTopic = e.Item.FindControl("hlTopic") as HyperLink;
                if (hlTopic != null)
                {
                    hlTopic.NavigateUrl = SEOHelper.GetForumTopicUrl(forumTopic);
                    hlTopic.Text = Server.HtmlEncode(forumTopic.Subject);
                }

                var hlTopicStarter = e.Item.FindControl("hlTopicStarter") as HyperLink;
                if(hlTopicStarter != null)
                {
                    if(customer != null && CustomerManager.AllowViewingProfiles && !customer.IsGuest)
                    {
                        hlTopicStarter.Text = Server.HtmlEncode(CustomerManager.FormatUserName(customer, true));
                        hlTopicStarter.NavigateUrl = SEOHelper.GetUserProfileUrl(customer.CustomerId);
                    }
                    else
                    {
                        hlTopicStarter.Visible = false;
                    }
                }

                var lblTopicStarter = e.Item.FindControl("lblTopicStarter") as Label;
                if(lblTopicStarter != null)
                {
                    if(customer != null && (!CustomerManager.AllowViewingProfiles || customer.IsGuest))
                    {
                        lblTopicStarter.Text = Server.HtmlEncode(CustomerManager.FormatUserName(customer, true));
                    }
                    else
                    {
                        lblTopicStarter.Visible = false;
                    }
                }
            }
        }

        public int ForumId
        {
            get
            {
                if (ViewState["ForumId"] == null)
                    return 0;
                else
                    return (int)ViewState["ForumId"];
            }
            set
            {
                this.ViewState["ForumId"] = value;
            }
        }

        public int TopicCount
        {
            get
            {
                if (ViewState["TopicCount"] == null)
                    return SettingManager.GetSettingValueInteger("Forums.ActiveDiscussions.TopicCount");
                else
                    return (int)ViewState["TopicCount"];
            }
            set
            {
                this.ViewState["TopicCount"] = value;
            }
        }

        public bool HideViewAllLink
        {
            get
            {
                if (ViewState["HideViewAllLink"] == null)
                    return false;
                else
                    return (bool)ViewState["HideViewAllLink"];
            }
            set
            {
                this.ViewState["HideViewAllLink"] = value;
            }
        }

    }
}

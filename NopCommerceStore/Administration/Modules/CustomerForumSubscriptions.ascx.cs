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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Xml;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CustomerForumSubscriptionsControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();
            base.OnPreRender(e);
        }

        protected string GetInfo(ForumSubscription subscription)
        {
            if (subscription == null)
            {
                return String.Empty;
            }

            Forum forum = subscription.Forum;
            if (forum != null)
            {
                return Server.HtmlEncode(forum.Name);
            }

            ForumTopic topic = subscription.Topic;
            if (topic != null)
            {
                return String.Format("{0}/{1}", Server.HtmlEncode(topic.Forum.Name), Server.HtmlEncode(topic.Subject)); ;
            }

            return String.Empty;
        }

        public void BindData()
        {
            gvForumSubscriptions.DataSource = ForumManager.GetAllSubscriptions(CustomerId, 0, 0, gvForumSubscriptions.PageSize, gvForumSubscriptions.PageIndex);
            gvForumSubscriptions.DataBind();
        }

        protected void gvForumSubscriptions_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvForumSubscriptions.PageIndex = e.NewPageIndex;
            BindData();
        }

        public int CustomerId
        {
            get
            {
                return CommonHelper.QueryStringInt("CustomerId");
            }
        }

        public void SaveInfo()
        {
            foreach (GridViewRow row in gvForumSubscriptions.Rows)
            {
                var cbSelect = row.FindControl("cbSelect") as CheckBox;
                var hfForumSubscriptionId = row.FindControl("hfForumSubscriptionId") as HiddenField;
                if (cbSelect != null && hfForumSubscriptionId != null)
                {
                    bool selected = cbSelect.Checked;
                    int forumSubscriptionId = int.Parse(hfForumSubscriptionId.Value);
                    if (selected)
                    {
                        ForumManager.DeleteSubscription(forumSubscriptionId);
                    }
                }
            }
        }
    }
}
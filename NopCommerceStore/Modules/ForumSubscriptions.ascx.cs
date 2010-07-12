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
    public partial class ForumSubscriptionsControl : BaseNopUserControl
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

            if (gvForumSubscriptions.Rows.Count > 0)
            {
                btnDeleteSelected.Visible = true;
            }
            else
            {
                btnDeleteSelected.Visible = false;
            }
            base.OnPreRender(e);
        }

        protected string GetForumTopicLink(ForumSubscription subscription)
        {
            if (subscription == null)
            {
                return String.Empty;
            }

            Forum forum = subscription.Forum;
            if (forum != null)
            {
                return SEOHelper.GetForumUrl(forum);
            }

            ForumTopic topic = subscription.Topic;
            if (topic != null)
            {
                return SEOHelper.GetForumTopicUrl(topic);
            }

            return String.Empty;
        }


        protected string GetForumTopicInfo(ForumSubscription subscription)
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
                return Server.HtmlEncode(topic.Subject);
            }

            return String.Empty;
        }

        public void BindData()
        {
            gvForumSubscriptions.DataBind();
        }

        protected void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    foreach (GridViewRow row in gvForumSubscriptions.Rows)
                    {
                        var cbSelect = row.FindControl("cbSelect") as CheckBox;
                        var hfForumSubscriptionId = row.FindControl("hfForumSubscriptionId") as HiddenField;
                        if (cbSelect != null && cbSelect.Checked && hfForumSubscriptionId != null)
                        {
                            int forumSubscriptionId = int.Parse(hfForumSubscriptionId.Value);
                            ForumSubscription subscription = ForumManager.GetSubscriptionById(forumSubscriptionId);

                            if (subscription != null && subscription.UserId == NopContext.Current.User.CustomerId)
                            {
                                ForumManager.DeleteSubscription(forumSubscriptionId);
                            }
                        }
                    }
                    gvForumSubscriptions.PageIndex = 0;
                    BindData();
                }
                catch (Exception exc)
                {
                    LogManager.InsertLog(LogTypeEnum.CustomerError, exc.Message, exc);
                }
            }
        }
    }
}
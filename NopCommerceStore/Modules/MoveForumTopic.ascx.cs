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
    public partial class MoveForumTopicControl : BaseNopUserControl
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
            pnlError.Visible = false;

            var forumTopic = ForumManager.GetTopicById(this.ForumTopicId);

            if (forumTopic == null)
            {
                Response.Redirect(SEOHelper.GetForumMainUrl());
            }

            if (!ForumManager.IsUserAllowedToMoveTopic(NopContext.Current.User, forumTopic))
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            ctrlForumBreadcrumb.ForumTopicId = forumTopic.ForumTopicId;
            ctrlForumBreadcrumb.BindData();

            ctrlForumSelector.SelectedForumId = forumTopic.ForumId;
            ctrlForumSelector.BindData();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var forumTopic = ForumManager.GetTopicById(this.ForumTopicId);
                if (forumTopic == null)
                {
                    Response.Redirect(SEOHelper.GetForumMainUrl());
                }

                ForumManager.MoveTopic(forumTopic.ForumTopicId, ctrlForumSelector.SelectedForumId);
                string topicURL = SEOHelper.GetForumTopicUrl(forumTopic);
                Response.Redirect(topicURL);
            }
            catch (Exception exc)
            {
                pnlError.Visible = true;
                lErrorMessage.Text = Server.HtmlEncode(exc.Message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                var forumTopic = ForumManager.GetTopicById(this.ForumTopicId);
                if (forumTopic != null)
                {
                    string topicUrl = SEOHelper.GetForumTopicUrl(forumTopic);
                    Response.Redirect(topicUrl);
                }
                else
                {
                    Response.Redirect(SEOHelper.GetForumMainUrl());
                }
            }
            catch (Exception exc)
            {
                pnlError.Visible = true;
                lErrorMessage.Text = Server.HtmlEncode(exc.Message);
            }
        }

        public int ForumTopicId
        {
            get
            {
                return CommonHelper.QueryStringInt("TopicId");
            }
        }
    }
}

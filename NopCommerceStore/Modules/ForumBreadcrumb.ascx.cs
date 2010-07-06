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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ForumBreadcrumbControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        public void BindData()
        {
            hlHome.NavigateUrl = CommonHelper.GetStoreLocation();
            hlForumsHome.NavigateUrl = SEOHelper.GetForumMainUrl();

            //topic
            var forumTopic = ForumManager.GetTopicById(this.ForumTopicId);
            if (forumTopic != null)
            {
                hlForumTopic.NavigateUrl = SEOHelper.GetForumTopicUrl(forumTopic);
                hlForumTopic.Text = Server.HtmlEncode(forumTopic.Subject);
            }
            else
            {
                phForumTopic.Visible = false;
            }

            //forum
            Forum forum = null;
            if (forumTopic != null)
            {
                forum = ForumManager.GetForumById(forumTopic.ForumId);
            }
            else
            {
                forum = ForumManager.GetForumById(this.ForumId);
            }

            if (forum != null)
            {
                hlForum.NavigateUrl = SEOHelper.GetForumUrl(forum);
                hlForum.Text = Server.HtmlEncode(forum.Name);
            }
            else
            {
                phForum.Visible = false;
            }

            //forum group
            ForumGroup forumGroup = null;
            if (forum != null)
            {
                forumGroup = ForumManager.GetForumGroupById(forum.ForumGroupId);
            }
            else
            {
                forumGroup = ForumManager.GetForumGroupById(this.ForumGroupId);
            }

            if (forumGroup != null)
            {
                hlForumGroup.NavigateUrl = SEOHelper.GetForumGroupUrl(forumGroup);
                hlForumGroup.Text = Server.HtmlEncode(forumGroup.Name);
            }
            else
            {
                phForumTopic.Visible = false;                
            }

        }

        public int ForumGroupId
        {
            get
            {
                if (ViewState["ForumGroupId"] == null)
                    return 0;
                else
                    return (int)ViewState["ForumGroupId"];
            }
            set
            {
                this.ViewState["ForumGroupId"] = value;
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

        public int ForumTopicId
        {
            get
            {
                if (ViewState["ForumTopicId"] == null)
                    return 0;
                else
                    return (int)ViewState["ForumTopicId"];
            }
            set
            {
                this.ViewState["ForumTopicId"] = value;
            }
        }
    }
}
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
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ForumTopicControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ctrlForumBreadcrumb.ForumTopicId = this.TopicId;
        }

        private void BindData()
        {
            var forumTopic = ForumManager.GetTopicById(this.TopicId, true);
            if (forumTopic != null)
            {
                btnEdit.Visible = ForumManager.IsUserAllowedToEditTopic(NopContext.Current.User, forumTopic);
                btnDelete.Visible = ForumManager.IsUserAllowedToDeleteTopic(NopContext.Current.User, forumTopic);
                btnDelete.OnClientClick = string.Format("return confirm('{0}')", GetLocaleResourceString("Common.AreYouSure"));
                btnMoveTopic.Visible = ForumManager.IsUserAllowedToMoveTopic(NopContext.Current.User, forumTopic);
                
                lblTopicSubject.Text = Server.HtmlEncode(forumTopic.Subject);

                int totalRecords = 0;
                int pageSize = 10;
                if (ForumManager.PostsPageSize > 0)
                {
                    pageSize = ForumManager.PostsPageSize;
                }

                var forumPosts = ForumManager.GetAllPosts(forumTopic.ForumTopicId, 0, string.Empty,
                    pageSize, this.CurrentPageIndex, out totalRecords);
                if (forumPosts.Count > 0)
                {
                    this.postsPager1.PageSize = pageSize;
                    this.postsPager1.TotalRecords = totalRecords;
                    this.postsPager1.PageIndex = this.CurrentPageIndex;

                    this.postsPager2.PageSize = pageSize;
                    this.postsPager2.TotalRecords = totalRecords;
                    this.postsPager2.PageIndex = this.CurrentPageIndex;

                    rptrPosts.DataSource = forumPosts;
                    rptrPosts.DataBind();
                }

                //subsciption
                if (ForumManager.IsUserAllowedToSubscribe(NopContext.Current.User))
                {
                    var forumSubscription = ForumManager.GetAllSubscriptions(NopContext.Current.User.CustomerId,
                        0, forumTopic.ForumTopicId, 1, 0).FirstOrDefault();

                    if (forumSubscription == null)
                    {
                        btnWatchTopic.Text = GetLocaleResourceString("Forum.WatchTopic");
                        btnWatchTopic2.Text = GetLocaleResourceString("Forum.WatchTopic");
                    }
                    else
                    {
                        btnWatchTopic.Text = GetLocaleResourceString("Forum.UnwatchTopic");
                        btnWatchTopic2.Text = GetLocaleResourceString("Forum.UnwatchTopic");
                    }
                }
                else
                {
                    btnWatchTopic.Visible = false;
                    btnWatchTopic2.Visible = false;
                }
            }
            else
            {
                Response.Redirect(SEOHelper.GetForumMainUrl());
            }
        }

        protected void btnWatchTopic_Click(object sender, EventArgs e)
        {
            var forumTopic = ForumManager.GetTopicById(this.TopicId);
            if (forumTopic == null)
                return;

            if (!ForumManager.IsUserAllowedToSubscribe(NopContext.Current.User))
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            var forumSubscription = ForumManager.GetAllSubscriptions(NopContext.Current.User.CustomerId,
                   0, forumTopic.ForumTopicId, 1, 0).FirstOrDefault();

            if (forumSubscription == null)
            {
                forumSubscription = ForumManager.InsertSubscription(Guid.NewGuid(),
                    NopContext.Current.User.CustomerId, 0, forumTopic.ForumTopicId, DateTime.UtcNow);
            }
            else
            {
                ForumManager.DeleteSubscription(forumSubscription.ForumSubscriptionId);
            }

            CommonHelper.ReloadCurrentPage();
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            var forumTopic = ForumManager.GetTopicById(this.TopicId);
            if (forumTopic != null)
            {
                if (!ForumManager.IsUserAllowedToEditTopic(NopContext.Current.User, forumTopic))
                {
                    string loginURL = SEOHelper.GetLoginPageUrl(true);
                    Response.Redirect(loginURL);
                }

                string editForumTopicURL = SEOHelper.GetEditForumTopicUrl(forumTopic.ForumTopicId);
                Response.Redirect(editForumTopicURL);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var forumTopic = ForumManager.GetTopicById(this.TopicId);
            if (forumTopic != null)
            {
                if (!ForumManager.IsUserAllowedToDeleteTopic(NopContext.Current.User, forumTopic))
                {
                    string loginURL = SEOHelper.GetLoginPageUrl(true);
                    Response.Redirect(loginURL);
                }

                ForumManager.DeleteTopic(forumTopic.ForumTopicId);

                string forumURL = SEOHelper.GetForumUrl(forumTopic.ForumId);
                Response.Redirect(forumURL);
            }
        }

        protected void btnMoveTopic_Click(object sender, EventArgs e)
        {
            var forumTopic = ForumManager.GetTopicById(this.TopicId);
            if (forumTopic != null)
            {
                if (!ForumManager.IsUserAllowedToMoveTopic(NopContext.Current.User, forumTopic))
                {
                    string loginURL = SEOHelper.GetLoginPageUrl(true);
                    Response.Redirect(loginURL);
                }

                string moveForumTopicURL = SEOHelper.GetMoveForumTopicUrl(forumTopic);
                Response.Redirect(moveForumTopicURL);
            }
        }

        protected void btnReply_Click(object sender, EventArgs e)
        {
            var forumTopic = ForumManager.GetTopicById(this.TopicId);
            if (forumTopic != null)
            {
                if(NopContext.Current.User == null && ForumManager.AllowGuestsToCreatePosts)
                {
                    CustomerManager.CreateAnonymousUser();
                }

                if (!ForumManager.IsUserAllowedToCreatePost(NopContext.Current.User, forumTopic))
                {
                    string loginURL = SEOHelper.GetLoginPageUrl(true);
                    Response.Redirect(loginURL);
                }

                string newForumPostURL = SEOHelper.GetNewForumPostUrl(forumTopic.ForumTopicId);
                Response.Redirect(newForumPostURL);
            }
        }

        public int CurrentPageIndex
        {
            get
            {
                int _pageIndex = CommonHelper.QueryStringInt(postsPager1.QueryStringProperty);
                _pageIndex--;
                if (_pageIndex < 0)
                    _pageIndex = 0;
                return _pageIndex;
            }
        }

        public int TopicId
        {
            get
            {
                return CommonHelper.QueryStringInt("TopicId");
            }
        }
    }
}

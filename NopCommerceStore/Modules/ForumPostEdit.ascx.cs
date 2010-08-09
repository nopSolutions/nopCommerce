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
using NopSolutions.NopCommerce.Common.Utils.Html;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ForumPostEditControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillDropDowns();
                BindData();
            }

            if (ForumManager.ForumEditor == EditorTypeEnum.BBCodeEditor)
            {
                LoadBBCodeEditorJS();
            }
        }

        private void LoadBBCodeEditorJS()
        {
            string bbCodeJS = "<script src='" + Page.ResolveUrl("~/editors/BBEditor/ed.js") + "' type='text/javascript'></script>";
            Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "BBCodeEditor", bbCodeJS, false);
        }

        private void FillDropDowns()
        {
            ddlPriority.Items.Clear();

            var ddlPriorityNormalItem = new ListItem(GetLocaleResourceString("Forum.Normal"), ((int)ForumTopicTypeEnum.Normal).ToString());
            ddlPriority.Items.Add(ddlPriorityNormalItem);

            var ddlPriorityStickyItem = new ListItem(GetLocaleResourceString("Forum.Sticky"), ((int)ForumTopicTypeEnum.Sticky).ToString());
            ddlPriority.Items.Add(ddlPriorityStickyItem);

            var ddlPriorityAnnouncementItem = new ListItem(GetLocaleResourceString("Forum.Announcement"), ((int)ForumTopicTypeEnum.Announcement).ToString());
            ddlPriority.Items.Add(ddlPriorityAnnouncementItem);
        }

        private void BindData()
        {
            pnlError.Visible = false;

            txtTopicBodySimple.Visible = false;
            txtTopicBodyBBCode.Visible = false;
            txtTopicBodyHtml.Visible = false;
            switch (ForumManager.ForumEditor)
            {
                case EditorTypeEnum.SimpleTextBox:
                    {
                        txtTopicBodySimple.Visible = true;
                        rfvTopicBody.ControlToValidate = "txtTopicBodySimple";
                    }
                    break;
                case EditorTypeEnum.BBCodeEditor:
                    {
                        txtTopicBodyBBCode.Visible = true;
                        rfvTopicBody.ControlToValidate = "txtTopicBodyBBCode";
                    }
                    break;
                case EditorTypeEnum.HtmlEditor:
                    {
                        txtTopicBodyHtml.Visible = true;
                        rfvTopicBody.Enabled = false;
                    }
                    break;
                default:
                    break;
            }

            if (this.AddTopic)
            {
                #region Adding topic

                var forum = ForumManager.GetForumById(this.ForumId);
                if (forum == null)
                {
                    Response.Redirect(SEOHelper.GetForumMainUrl());
                }

                if(NopContext.Current.User == null && ForumManager.AllowGuestsToCreateTopics)
                {
                    CustomerManager.CreateAnonymousUser();
                }

                if (!ForumManager.IsUserAllowedToCreateTopic(NopContext.Current.User, forum))
                {
                    string loginURL = SEOHelper.GetLoginPageUrl(true);
                    Response.Redirect(loginURL);
                }

                lblTitle.Text = GetLocaleResourceString("Forum.NewTopic");
                phForumName.Visible = true;
                lblForumName.Text = Server.HtmlEncode(forum.Name);
                txtTopicTitle.Visible = true;
                txtTopicTitle.Text = string.Empty;
                lblTopicTitle.Visible = false;
                lblTopicTitle.Text = string.Empty;

                ctrlForumBreadcrumb.ForumId = forum.ForumId;
                ctrlForumBreadcrumb.BindData();

                phPriority.Visible = ForumManager.IsUserAllowedToSetTopicPriority(NopContext.Current.User);
                phSubscribe.Visible = ForumManager.IsUserAllowedToSubscribe(NopContext.Current.User);

                #endregion
            }
            else if (this.EditTopic)
            {
                #region Editing topic
                var forumTopic = ForumManager.GetTopicById(this.ForumTopicId);

                if (forumTopic == null)
                {
                    Response.Redirect(SEOHelper.GetForumMainUrl());
                }

                if (!ForumManager.IsUserAllowedToEditTopic(NopContext.Current.User, forumTopic))
                {
                    string loginURL = SEOHelper.GetLoginPageUrl(true);
                    Response.Redirect(loginURL);
                }

                var forum = forumTopic.Forum;
                if (forum == null)
                {
                    Response.Redirect(SEOHelper.GetForumMainUrl());
                }

                lblTitle.Text = GetLocaleResourceString("Forum.EditTopic");
                phForumName.Visible = true;
                lblForumName.Text = Server.HtmlEncode(forum.Name);
                txtTopicTitle.Visible = true;
                txtTopicTitle.Text = forumTopic.Subject;
                lblTopicTitle.Visible = false;
                lblTopicTitle.Text = string.Empty;

                ctrlForumBreadcrumb.ForumTopicId = forumTopic.ForumTopicId;
                ctrlForumBreadcrumb.BindData();
                
                CommonHelper.SelectListItem(this.ddlPriority, forumTopic.TopicTypeId);

                var firstPost = forumTopic.FirstPost;
                if (firstPost != null)
                {
                    switch (ForumManager.ForumEditor)
                    {
                        case EditorTypeEnum.SimpleTextBox:
                            {
                                txtTopicBodySimple.Text = firstPost.Text;
                            }
                            break;
                        case EditorTypeEnum.BBCodeEditor:
                            {
                                txtTopicBodyBBCode.Text = firstPost.Text;
                            }
                            break;
                        case EditorTypeEnum.HtmlEditor:
                            {
                                txtTopicBodyHtml.Content = firstPost.Text;
                            }
                            break;
                        default:
                            break;
                    }
                }


                phPriority.Visible = ForumManager.IsUserAllowedToSetTopicPriority(NopContext.Current.User);
                //subscription
                if (ForumManager.IsUserAllowedToSubscribe(NopContext.Current.User.CustomerId))
                {
                    phSubscribe.Visible = true;
                    var forumSubscription = ForumManager.GetAllSubscriptions(NopContext.Current.User.CustomerId,
                        0, forumTopic.ForumTopicId, 1, 0).FirstOrDefault();
                    cbSubscribe.Checked = forumSubscription != null;
                }
                else
                {
                    phSubscribe.Visible = false;
                }
                #endregion
            }
            else if (this.AddPost)
            {
                #region Adding post

                var forumTopic = ForumManager.GetTopicById(this.ForumTopicId);
                if (forumTopic == null)
                {
                    Response.Redirect(SEOHelper.GetForumMainUrl());
                }

                if(NopContext.Current.User == null && ForumManager.AllowGuestsToCreatePosts)
                {
                    CustomerManager.CreateAnonymousUser();
                }

                if (!ForumManager.IsUserAllowedToCreatePost(NopContext.Current.User, forumTopic))
                {
                    string loginURL = SEOHelper.GetLoginPageUrl(true);
                    Response.Redirect(loginURL);
                }

                ctrlForumBreadcrumb.ForumTopicId = forumTopic.ForumTopicId;
                ctrlForumBreadcrumb.BindData();

                lblTitle.Text = GetLocaleResourceString("Forum.NewPost");
                phForumName.Visible = false;
                lblForumName.Text = string.Empty;
                txtTopicTitle.Visible = false;
                txtTopicTitle.Text = string.Empty;
                lblTopicTitle.Visible = true;
                lblTopicTitle.Text = Server.HtmlEncode(forumTopic.Subject);

                var quotePost = ForumManager.GetPostById(QuotePostId);
                if(quotePost != null && quotePost.TopicId == forumTopic.ForumTopicId)
                {
                    switch(ForumManager.ForumEditor)
                    {
                        case EditorTypeEnum.SimpleTextBox:
                            txtTopicBodySimple.Text = String.Format("{0}:\n{1}\n", CustomerManager.FormatUserName(quotePost.User), quotePost.Text);
                            break;
                        case EditorTypeEnum.BBCodeEditor:
                            txtTopicBodyBBCode.Text = String.Format("[quote={0}]{1}[/quote]", CustomerManager.FormatUserName(quotePost.User), BBCodeHelper.RemoveQuotes(quotePost.Text));
                            break;
                        case EditorTypeEnum.HtmlEditor:
                            txtTopicBodyHtml.Content = String.Format("<b>{0}:</b><p style=\"padding: 5px 5px 5px 5px; border: dashed 1px black; background-color: #ffffff;\">{1}</p>", CustomerManager.FormatUserName(quotePost.User), quotePost.Text);
                            break;
                    }
                }

                phPriority.Visible = false;
                //subscription
                if (ForumManager.IsUserAllowedToSubscribe(NopContext.Current.User))
                {
                    phSubscribe.Visible = true;
                    var forumSubscription = ForumManager.GetAllSubscriptions(NopContext.Current.User.CustomerId,
                        0, forumTopic.ForumTopicId, 1, 0).FirstOrDefault();
                    cbSubscribe.Checked = forumSubscription != null;
                }
                else
                {
                    phSubscribe.Visible = false;
                }
                #endregion
            }
            else if (this.EditPost)
            {
                #region Editing post

                var forumPost = ForumManager.GetPostById(this.ForumPostId);

                if (forumPost == null)
                {
                    Response.Redirect(SEOHelper.GetForumMainUrl());
                }

                if (!ForumManager.IsUserAllowedToEditPost(NopContext.Current.User, forumPost))
                {
                    string loginURL = SEOHelper.GetLoginPageUrl(true);
                    Response.Redirect(loginURL);
                }

                var forumTopic = forumPost.Topic;
                if (forumTopic == null)
                {
                    Response.Redirect(SEOHelper.GetForumMainUrl());
                }

                lblTitle.Text = GetLocaleResourceString("Forum.EditPost");
                phForumName.Visible = false;
                lblForumName.Text = string.Empty;
                txtTopicTitle.Visible = false;
                txtTopicTitle.Text = string.Empty;
                lblTopicTitle.Visible = true;
                lblTopicTitle.Text = Server.HtmlEncode(forumTopic.Subject);

                ctrlForumBreadcrumb.ForumTopicId = forumTopic.ForumTopicId;
                ctrlForumBreadcrumb.BindData();


                switch (ForumManager.ForumEditor)
                {
                    case EditorTypeEnum.SimpleTextBox:
                        {
                            txtTopicBodySimple.Text = forumPost.Text;
                        }
                        break;
                    case EditorTypeEnum.BBCodeEditor:
                        {
                            txtTopicBodyBBCode.Text = forumPost.Text;
                        }
                        break;
                    case EditorTypeEnum.HtmlEditor:
                        {
                            txtTopicBodyHtml.Content = forumPost.Text;
                        }
                        break;
                    default:
                        break;
                }

                phPriority.Visible = false;
                //subscription
                if (ForumManager.IsUserAllowedToSubscribe(NopContext.Current.User.CustomerId))
                {
                    phSubscribe.Visible = true;
                    var forumSubscription = ForumManager.GetAllSubscriptions(NopContext.Current.User.CustomerId,
                        0, forumTopic.ForumTopicId, 1, 0).FirstOrDefault();
                    cbSubscribe.Checked = forumSubscription != null;
                }
                else
                {
                    phSubscribe.Visible = false;
                }
                #endregion
            }
            else
            {
                Response.Redirect(SEOHelper.GetForumMainUrl());
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string text = string.Empty;

                switch (ForumManager.ForumEditor)
                {
                    case EditorTypeEnum.SimpleTextBox:
                        {
                            text = txtTopicBodySimple.Text.Trim();
                        }
                        break;
                    case EditorTypeEnum.BBCodeEditor:
                        {
                            text = txtTopicBodyBBCode.Text.Trim();
                        }
                        break;
                    case EditorTypeEnum.HtmlEditor:
                        {
                            text = txtTopicBodyHtml.Content;
                        }
                        break;
                    default:
                        break;
                }
                
                string subject = txtTopicTitle.Text;
                var topicType = ForumTopicTypeEnum.Normal;
                bool subscribe = cbSubscribe.Checked;

                string IPAddress = NopContext.Current.UserHostAddress;

                DateTime nowDT = DateTime.UtcNow;

                if (ForumManager.IsUserAllowedToSetTopicPriority(NopContext.Current.User))
                {
                    topicType = (ForumTopicTypeEnum)Enum.ToObject(typeof(ForumTopicTypeEnum), int.Parse(ddlPriority.SelectedItem.Value));
                }

                text = text.Trim();
                if (String.IsNullOrEmpty(text))
                    throw new NopException(GetLocaleResourceString("Forum.TextCannotBeEmpty"));

                if (this.AddTopic)
                {
                    #region Adding topic
                    var forum = ForumManager.GetForumById(this.ForumId);
                    if (forum == null)
                    {
                        Response.Redirect(SEOHelper.GetForumMainUrl());
                    }

                    if (!ForumManager.IsUserAllowedToCreateTopic(NopContext.Current.User, forum))
                    {
                        string loginURL = SEOHelper.GetLoginPageUrl(true);
                        Response.Redirect(loginURL);
                    }
                    
                    subject = subject.Trim();
                    if (String.IsNullOrEmpty(subject))
                        throw new NopException(GetLocaleResourceString("Forum.TopicSubjectCannotBeEmpty"));

                    var forumTopic = ForumManager.InsertTopic(forum.ForumId, NopContext.Current.User.CustomerId,
                        topicType, subject, 0, 0, 0, 0, null, nowDT, nowDT, true);

                    var forumPost = ForumManager.InsertPost(forumTopic.ForumTopicId, NopContext.Current.User.CustomerId,
                        text, IPAddress, nowDT, nowDT, false);

                    forumTopic = ForumManager.UpdateTopic(forumTopic.ForumTopicId, forumTopic.ForumId,
                        forumTopic.UserId, forumTopic.TopicType, forumTopic.Subject, 1,
                        0, forumPost.ForumPostId, forumTopic.UserId,
                        forumPost.CreatedOn, forumTopic.CreatedOn, nowDT);

                    //subscription
                    if (ForumManager.IsUserAllowedToSubscribe(NopContext.Current.User))
                    {
                        if (subscribe)
                        {
                            var forumSubscription = ForumManager.InsertSubscription(Guid.NewGuid(),
                                NopContext.Current.User.CustomerId, 0, forumTopic.ForumTopicId, nowDT);
                        }
                    }

                    string topicURL = SEOHelper.GetForumTopicUrl(forumTopic);
                    Response.Redirect(topicURL);
                    #endregion
                }
                else if (this.EditTopic)
                {
                    #region Editing topic
                    var forumTopic = ForumManager.GetTopicById(this.ForumTopicId);
                    if (forumTopic == null)
                    {
                        Response.Redirect(SEOHelper.GetForumMainUrl());
                    }

                    if (!ForumManager.IsUserAllowedToEditTopic(NopContext.Current.User, forumTopic))
                    {
                        string loginURL = SEOHelper.GetLoginPageUrl(true);
                        Response.Redirect(loginURL);
                    }

                    subject = subject.Trim();
                    if (String.IsNullOrEmpty(subject))
                        throw new NopException(GetLocaleResourceString("Forum.TopicSubjectCannotBeEmpty"));

                    forumTopic = ForumManager.UpdateTopic(forumTopic.ForumTopicId, forumTopic.ForumId,
                        forumTopic.UserId, topicType, subject, forumTopic.NumPosts,
                        forumTopic.Views, forumTopic.LastPostId, forumTopic.LastPostUserId,
                        forumTopic.LastPostTime, forumTopic.CreatedOn, nowDT);

                    var firstPost = forumTopic.FirstPost;
                    if (firstPost != null)
                    {
                        firstPost = ForumManager.UpdatePost(firstPost.ForumPostId, firstPost.TopicId,
                            firstPost.UserId, text, firstPost.IPAddress, firstPost.CreatedOn, nowDT);
                    }
                    else
                    {
                        //error
                        firstPost = ForumManager.InsertPost(forumTopic.ForumTopicId,
                            forumTopic.UserId, text, IPAddress, forumTopic.CreatedOn, nowDT, false);
                    }

                    //subscription
                    if (ForumManager.IsUserAllowedToSubscribe(NopContext.Current.User.CustomerId))
                    {
                        var forumSubscription = ForumManager.GetAllSubscriptions(NopContext.Current.User.CustomerId,
                            0, forumTopic.ForumTopicId, 1, 0).FirstOrDefault();
                        if (subscribe)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = ForumManager.InsertSubscription(Guid.NewGuid(),
                                    NopContext.Current.User.CustomerId, 0, forumTopic.ForumTopicId, nowDT);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                ForumManager.DeleteSubscription(forumSubscription.ForumSubscriptionId);
                            }
                        }
                    }

                    string topicURL = SEOHelper.GetForumTopicUrl(forumTopic);
                    Response.Redirect(topicURL);
                    #endregion
                }
                else if (this.AddPost)
                {
                    #region Adding post
                    var forumTopic = ForumManager.GetTopicById(this.ForumTopicId);
                    if (forumTopic == null)
                    {
                        Response.Redirect(SEOHelper.GetForumMainUrl());
                    }

                    if (!ForumManager.IsUserAllowedToCreatePost(NopContext.Current.User, forumTopic))
                    {
                        string loginURL = SEOHelper.GetLoginPageUrl(true);
                        Response.Redirect(loginURL);
                    }

                    var forumPost = ForumManager.InsertPost(this.ForumTopicId, NopContext.Current.User.CustomerId,
                        text, IPAddress, nowDT, nowDT, true);

                    //subscription
                    if (ForumManager.IsUserAllowedToSubscribe(NopContext.Current.User.CustomerId))
                    {
                        var forumSubscription = ForumManager.GetAllSubscriptions(NopContext.Current.User.CustomerId,
                            0, forumPost.TopicId, 1, 0).FirstOrDefault();
                        if (subscribe)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = ForumManager.InsertSubscription(Guid.NewGuid(),
                                    NopContext.Current.User.CustomerId, 0, forumPost.TopicId, nowDT);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                ForumManager.DeleteSubscription(forumSubscription.ForumSubscriptionId);
                            }
                        }
                    }
                    

                    int pageSize = 10;
                    if (ForumManager.PostsPageSize > 0)
                    {
                        pageSize = ForumManager.PostsPageSize;
                    }
                    int pageIndex = ForumManager.CalculateTopicPageIndex(forumPost.TopicId, pageSize, forumPost.ForumPostId);
                    string topicURL = SEOHelper.GetForumTopicUrl(forumPost.TopicId, "p", pageIndex + 1, forumPost.ForumPostId);
                    Response.Redirect(topicURL);
                    #endregion
                }
                else if (this.EditPost)
                {
                    #region Editing post
                    var forumPost = ForumManager.GetPostById(this.ForumPostId);
                    if (forumPost == null)
                    {
                        Response.Redirect(SEOHelper.GetForumMainUrl());
                    }

                    if (!ForumManager.IsUserAllowedToEditPost(NopContext.Current.User, forumPost))
                    {
                        string loginURL = SEOHelper.GetLoginPageUrl(true);
                        Response.Redirect(loginURL);
                    }

                    forumPost = ForumManager.UpdatePost(forumPost.ForumPostId, forumPost.TopicId,
                        forumPost.UserId, text, forumPost.IPAddress, forumPost.CreatedOn, nowDT);

                    //subscription
                    if (ForumManager.IsUserAllowedToSubscribe(NopContext.Current.User.CustomerId))
                    {
                        var forumSubscription = ForumManager.GetAllSubscriptions(NopContext.Current.User.CustomerId,
                            0, forumPost.TopicId, 1, 0).FirstOrDefault();
                        if (subscribe)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = ForumManager.InsertSubscription(Guid.NewGuid(),
                                    NopContext.Current.User.CustomerId, 0, forumPost.TopicId, nowDT);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                ForumManager.DeleteSubscription(forumSubscription.ForumSubscriptionId);
                            }
                        }
                    }

                    int pageSize = 10;
                    if (ForumManager.PostsPageSize > 0)
                    {
                        pageSize = ForumManager.PostsPageSize;
                    }
                    int pageIndex = ForumManager.CalculateTopicPageIndex(forumPost.TopicId, pageSize, forumPost.ForumPostId);
                    string topicURL = SEOHelper.GetForumTopicUrl(forumPost.TopicId, "p", pageIndex + 1, forumPost.ForumPostId);
                    Response.Redirect(topicURL);
                    #endregion
                }
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
                if (this.AddTopic)
                {
                    var forum = ForumManager.GetForumById(this.ForumId);
                    if (forum != null)
                    {
                        string forumUrl = SEOHelper.GetForumUrl(forum);
                        Response.Redirect(forumUrl);
                    }
                    else
                    {
                        Response.Redirect(SEOHelper.GetForumMainUrl());
                    }
                }
                else if (this.EditTopic)
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
                else if (this.AddPost)
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
                else if (this.EditPost)
                {
                    var forumPost = ForumManager.GetPostById(this.ForumPostId);
                    if (forumPost != null)
                    {
                        string topicUrl = SEOHelper.GetForumTopicUrl(forumPost.TopicId);
                        Response.Redirect(topicUrl);
                    }
                    else
                    {
                        Response.Redirect(SEOHelper.GetForumMainUrl());
                    }
                }
            }
            catch (Exception exc)
            {
                pnlError.Visible = true;
                lErrorMessage.Text = Server.HtmlEncode(exc.Message);
            }
        }

        [DefaultValue(true)]
        public bool AddTopic
        {
            get
            {
                object obj2 = this.ViewState["AddTopic"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["AddTopic"] = value;
            }
        }

        public bool EditTopic
        {
            get
            {
                object obj2 = this.ViewState["EditTopic"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["EditTopic"] = value;
            }
        }

        public bool AddPost
        {
            get
            {
                object obj2 = this.ViewState["AddPost"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["AddPost"] = value;
            }
        }

        public bool EditPost
        {
            get
            {
                object obj2 = this.ViewState["EditPost"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["EditPost"] = value;
            }
        }

        public int ForumPostId
        {
            get
            {
                return CommonHelper.QueryStringInt("PostId");
            }
        }

        public int QuotePostId
        {
            get
            {
                return CommonHelper.QueryStringInt("QuotePostId");
            }
        }

        public int ForumTopicId
        {
            get
            {
                return CommonHelper.QueryStringInt("TopicId");
            }
        }

        public int ForumId
        {
            get
            {
                return CommonHelper.QueryStringInt("ForumId");
            }
        }
    }
}

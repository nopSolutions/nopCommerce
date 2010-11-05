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
using NopSolutions.NopCommerce.BusinessLogic.IoC;

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

            if (IoCFactory.Resolve<IForumService>().ForumEditor == EditorTypeEnum.BBCodeEditor)
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
            switch (IoCFactory.Resolve<IForumService>().ForumEditor)
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

                var forum = IoCFactory.Resolve<IForumService>().GetForumById(this.ForumId);
                if (forum == null)
                {
                    Response.Redirect(SEOHelper.GetForumMainUrl());
                }

                if(NopContext.Current.User == null && IoCFactory.Resolve<IForumService>().AllowGuestsToCreateTopics)
                {
                    IoCFactory.Resolve<ICustomerService>().CreateAnonymousUser();
                }

                if (!IoCFactory.Resolve<IForumService>().IsUserAllowedToCreateTopic(NopContext.Current.User, forum))
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

                phPriority.Visible = IoCFactory.Resolve<IForumService>().IsUserAllowedToSetTopicPriority(NopContext.Current.User);
                phSubscribe.Visible = IoCFactory.Resolve<IForumService>().IsUserAllowedToSubscribe(NopContext.Current.User);

                #endregion
            }
            else if (this.EditTopic)
            {
                #region Editing topic
                var forumTopic = IoCFactory.Resolve<IForumService>().GetTopicById(this.ForumTopicId);

                if (forumTopic == null)
                {
                    Response.Redirect(SEOHelper.GetForumMainUrl());
                }

                if (!IoCFactory.Resolve<IForumService>().IsUserAllowedToEditTopic(NopContext.Current.User, forumTopic))
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
                    switch (IoCFactory.Resolve<IForumService>().ForumEditor)
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
                                txtTopicBodyHtml.Value = firstPost.Text;
                            }
                            break;
                        default:
                            break;
                    }
                }


                phPriority.Visible = IoCFactory.Resolve<IForumService>().IsUserAllowedToSetTopicPriority(NopContext.Current.User);
                //subscription
                if (IoCFactory.Resolve<IForumService>().IsUserAllowedToSubscribe(NopContext.Current.User.CustomerId))
                {
                    phSubscribe.Visible = true;
                    var forumSubscription = IoCFactory.Resolve<IForumService>().GetAllSubscriptions(NopContext.Current.User.CustomerId,
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

                var forumTopic = IoCFactory.Resolve<IForumService>().GetTopicById(this.ForumTopicId);
                if (forumTopic == null)
                {
                    Response.Redirect(SEOHelper.GetForumMainUrl());
                }

                if(NopContext.Current.User == null && IoCFactory.Resolve<IForumService>().AllowGuestsToCreatePosts)
                {
                    IoCFactory.Resolve<ICustomerService>().CreateAnonymousUser();
                }

                if (!IoCFactory.Resolve<IForumService>().IsUserAllowedToCreatePost(NopContext.Current.User, forumTopic))
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

                var quotePost = IoCFactory.Resolve<IForumService>().GetPostById(QuotePostId);
                if(quotePost != null && quotePost.TopicId == forumTopic.ForumTopicId)
                {
                    switch(IoCFactory.Resolve<IForumService>().ForumEditor)
                    {
                        case EditorTypeEnum.SimpleTextBox:
                            txtTopicBodySimple.Text = String.Format("{0}:\n{1}\n", IoCFactory.Resolve<ICustomerService>().FormatUserName(quotePost.User), quotePost.Text);
                            break;
                        case EditorTypeEnum.BBCodeEditor:
                            txtTopicBodyBBCode.Text = String.Format("[quote={0}]{1}[/quote]", IoCFactory.Resolve<ICustomerService>().FormatUserName(quotePost.User), BBCodeHelper.RemoveQuotes(quotePost.Text));
                            break;
                        case EditorTypeEnum.HtmlEditor:
                            txtTopicBodyHtml.Value = String.Format("<b>{0}:</b><p style=\"padding: 5px 5px 5px 5px; border: dashed 1px black; background-color: #ffffff;\">{1}</p>", IoCFactory.Resolve<ICustomerService>().FormatUserName(quotePost.User), quotePost.Text);
                            break;
                    }
                }

                phPriority.Visible = false;
                //subscription
                if (IoCFactory.Resolve<IForumService>().IsUserAllowedToSubscribe(NopContext.Current.User))
                {
                    phSubscribe.Visible = true;
                    var forumSubscription = IoCFactory.Resolve<IForumService>().GetAllSubscriptions(NopContext.Current.User.CustomerId,
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

                var forumPost = IoCFactory.Resolve<IForumService>().GetPostById(this.ForumPostId);

                if (forumPost == null)
                {
                    Response.Redirect(SEOHelper.GetForumMainUrl());
                }

                if (!IoCFactory.Resolve<IForumService>().IsUserAllowedToEditPost(NopContext.Current.User, forumPost))
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


                switch (IoCFactory.Resolve<IForumService>().ForumEditor)
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
                            txtTopicBodyHtml.Value = forumPost.Text;
                        }
                        break;
                    default:
                        break;
                }

                phPriority.Visible = false;
                //subscription
                if (IoCFactory.Resolve<IForumService>().IsUserAllowedToSubscribe(NopContext.Current.User.CustomerId))
                {
                    phSubscribe.Visible = true;
                    var forumSubscription = IoCFactory.Resolve<IForumService>().GetAllSubscriptions(NopContext.Current.User.CustomerId,
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

                switch (IoCFactory.Resolve<IForumService>().ForumEditor)
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
                            text = txtTopicBodyHtml.Value;
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

                if (IoCFactory.Resolve<IForumService>().IsUserAllowedToSetTopicPriority(NopContext.Current.User))
                {
                    topicType = (ForumTopicTypeEnum)Enum.ToObject(typeof(ForumTopicTypeEnum), int.Parse(ddlPriority.SelectedItem.Value));
                }

                text = text.Trim();
                if (String.IsNullOrEmpty(text))
                    throw new NopException(GetLocaleResourceString("Forum.TextCannotBeEmpty"));

                if (this.AddTopic)
                {
                    #region Adding topic
                    var forum = IoCFactory.Resolve<IForumService>().GetForumById(this.ForumId);
                    if (forum == null)
                    {
                        Response.Redirect(SEOHelper.GetForumMainUrl());
                    }

                    if (!IoCFactory.Resolve<IForumService>().IsUserAllowedToCreateTopic(NopContext.Current.User, forum))
                    {
                        string loginURL = SEOHelper.GetLoginPageUrl(true);
                        Response.Redirect(loginURL);
                    }
                    
                    subject = subject.Trim();
                    if (String.IsNullOrEmpty(subject))
                        throw new NopException(GetLocaleResourceString("Forum.TopicSubjectCannotBeEmpty"));

                    //forum topic
                    var forumTopic = new ForumTopic()
                    {
                        ForumId = forum.ForumId,
                        UserId = NopContext.Current.User.CustomerId,
                        TopicTypeId = (int)topicType,
                        Subject = subject,
                        CreatedOn = nowDT,
                        UpdatedOn = nowDT
                    };
                    IoCFactory.Resolve<IForumService>().InsertTopic(forumTopic, true);

                    //forum post
                    var forumPost = new ForumPost()
                    {
                        TopicId = forumTopic.ForumTopicId,
                        UserId = NopContext.Current.User.CustomerId,
                        Text = text,
                        IPAddress = IPAddress,
                        CreatedOn = nowDT,
                        UpdatedOn = nowDT
                    };
                    IoCFactory.Resolve<IForumService>().InsertPost(forumPost, false);

                    //update forum topic
                    forumTopic.NumPosts = 1;
                    forumTopic.LastPostId = forumPost.ForumPostId;
                    forumTopic.LastPostUserId = forumPost.UserId;
                    forumTopic.LastPostTime = forumPost.CreatedOn;
                    forumTopic.UpdatedOn = nowDT;
                    IoCFactory.Resolve<IForumService>().UpdateTopic(forumTopic);

                    //subscription
                    if (IoCFactory.Resolve<IForumService>().IsUserAllowedToSubscribe(NopContext.Current.User))
                    {
                        if (subscribe)
                        {
                            var forumSubscription = new ForumSubscription()
                            {
                                SubscriptionGuid = Guid.NewGuid(),
                                UserId = NopContext.Current.User.CustomerId,
                                TopicId = forumTopic.ForumTopicId,
                                CreatedOn = nowDT
                            };

                            IoCFactory.Resolve<IForumService>().InsertSubscription(forumSubscription);
                        }
                    }

                    string topicURL = SEOHelper.GetForumTopicUrl(forumTopic);
                    Response.Redirect(topicURL);
                    #endregion
                }
                else if (this.EditTopic)
                {
                    #region Editing topic
                    var forumTopic = IoCFactory.Resolve<IForumService>().GetTopicById(this.ForumTopicId);
                    if (forumTopic == null)
                    {
                        Response.Redirect(SEOHelper.GetForumMainUrl());
                    }

                    if (!IoCFactory.Resolve<IForumService>().IsUserAllowedToEditTopic(NopContext.Current.User, forumTopic))
                    {
                        string loginURL = SEOHelper.GetLoginPageUrl(true);
                        Response.Redirect(loginURL);
                    }

                    subject = subject.Trim();
                    if (String.IsNullOrEmpty(subject))
                        throw new NopException(GetLocaleResourceString("Forum.TopicSubjectCannotBeEmpty"));

                    //forum topic
                    forumTopic.TopicTypeId = (int)topicType;
                    forumTopic.Subject = subject;
                    forumTopic.UpdatedOn = nowDT;
                    IoCFactory.Resolve<IForumService>().UpdateTopic(forumTopic);

                    //forum post
                    var firstPost = forumTopic.FirstPost;
                    if (firstPost != null)
                    {
                        firstPost.Text = text;
                        firstPost.UpdatedOn = nowDT;
                        IoCFactory.Resolve<IForumService>().UpdatePost(firstPost);
                    }
                    else
                    {
                        //error (not possible)
                        firstPost = new ForumPost()
                        {
                            TopicId = forumTopic.ForumTopicId,
                            UserId = forumTopic.UserId,
                            Text = text,
                            IPAddress = IPAddress,
                            UpdatedOn = nowDT
                        };

                        IoCFactory.Resolve<IForumService>().InsertPost(firstPost, false);
                    }

                    //subscription
                    if (IoCFactory.Resolve<IForumService>().IsUserAllowedToSubscribe(NopContext.Current.User.CustomerId))
                    {
                        var forumSubscription = IoCFactory.Resolve<IForumService>().GetAllSubscriptions(NopContext.Current.User.CustomerId,
                            0, forumTopic.ForumTopicId, 1, 0).FirstOrDefault();
                        if (subscribe)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription()
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    UserId = NopContext.Current.User.CustomerId,
                                    TopicId = forumTopic.ForumTopicId,
                                    CreatedOn = nowDT
                                };

                                IoCFactory.Resolve<IForumService>().InsertSubscription(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                IoCFactory.Resolve<IForumService>().DeleteSubscription(forumSubscription.ForumSubscriptionId);
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
                    var forumTopic = IoCFactory.Resolve<IForumService>().GetTopicById(this.ForumTopicId);
                    if (forumTopic == null)
                    {
                        Response.Redirect(SEOHelper.GetForumMainUrl());
                    }

                    if (!IoCFactory.Resolve<IForumService>().IsUserAllowedToCreatePost(NopContext.Current.User, forumTopic))
                    {
                        string loginURL = SEOHelper.GetLoginPageUrl(true);
                        Response.Redirect(loginURL);
                    }

                    //forum post
                    var forumPost = new ForumPost()
                    {
                        TopicId = this.ForumTopicId,
                        UserId = NopContext.Current.User.CustomerId,
                        Text = text,
                        IPAddress = IPAddress,
                        CreatedOn = nowDT,
                        UpdatedOn = nowDT
                    };
                    IoCFactory.Resolve<IForumService>().InsertPost(forumPost, true);

                    //subscription
                    if (IoCFactory.Resolve<IForumService>().IsUserAllowedToSubscribe(NopContext.Current.User.CustomerId))
                    {
                        var forumSubscription = IoCFactory.Resolve<IForumService>().GetAllSubscriptions(NopContext.Current.User.CustomerId,
                            0, forumPost.TopicId, 1, 0).FirstOrDefault();
                        if (subscribe)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription()
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    UserId = NopContext.Current.User.CustomerId,
                                    TopicId = forumPost.TopicId,
                                    CreatedOn = nowDT
                                };

                                 IoCFactory.Resolve<IForumService>().InsertSubscription(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                IoCFactory.Resolve<IForumService>().DeleteSubscription(forumSubscription.ForumSubscriptionId);
                            }
                        }
                    }
                    

                    int pageSize = 10;
                    if (IoCFactory.Resolve<IForumService>().PostsPageSize > 0)
                    {
                        pageSize = IoCFactory.Resolve<IForumService>().PostsPageSize;
                    }
                    int pageIndex = IoCFactory.Resolve<IForumService>().CalculateTopicPageIndex(forumPost.TopicId, pageSize, forumPost.ForumPostId);
                    string topicURL = SEOHelper.GetForumTopicUrl(forumPost.TopicId, "p", pageIndex + 1, forumPost.ForumPostId);
                    Response.Redirect(topicURL);
                    #endregion
                }
                else if (this.EditPost)
                {
                    #region Editing post
                    var forumPost = IoCFactory.Resolve<IForumService>().GetPostById(this.ForumPostId);
                    if (forumPost == null)
                    {
                        Response.Redirect(SEOHelper.GetForumMainUrl());
                    }

                    if (!IoCFactory.Resolve<IForumService>().IsUserAllowedToEditPost(NopContext.Current.User, forumPost))
                    {
                        string loginURL = SEOHelper.GetLoginPageUrl(true);
                        Response.Redirect(loginURL);
                    }

                    forumPost.Text = text;
                    forumPost.UpdatedOn = nowDT;
                    IoCFactory.Resolve<IForumService>().UpdatePost(forumPost);

                    //subscription
                    if (IoCFactory.Resolve<IForumService>().IsUserAllowedToSubscribe(NopContext.Current.User.CustomerId))
                    {
                        var forumSubscription = IoCFactory.Resolve<IForumService>().GetAllSubscriptions(NopContext.Current.User.CustomerId,
                            0, forumPost.TopicId, 1, 0).FirstOrDefault();
                        if (subscribe)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription()
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    UserId = NopContext.Current.User.CustomerId,
                                    TopicId = forumPost.TopicId,
                                    CreatedOn = nowDT
                                };

                                IoCFactory.Resolve<IForumService>().InsertSubscription(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                IoCFactory.Resolve<IForumService>().DeleteSubscription(forumSubscription.ForumSubscriptionId);
                            }
                        }
                    }

                    int pageSize = 10;
                    if (IoCFactory.Resolve<IForumService>().PostsPageSize > 0)
                    {
                        pageSize = IoCFactory.Resolve<IForumService>().PostsPageSize;
                    }
                    int pageIndex = IoCFactory.Resolve<IForumService>().CalculateTopicPageIndex(forumPost.TopicId, pageSize, forumPost.ForumPostId);
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
                    var forum = IoCFactory.Resolve<IForumService>().GetForumById(this.ForumId);
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
                    var forumTopic = IoCFactory.Resolve<IForumService>().GetTopicById(this.ForumTopicId);
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
                    var forumTopic = IoCFactory.Resolve<IForumService>().GetTopicById(this.ForumTopicId);
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
                    var forumPost = IoCFactory.Resolve<IForumService>().GetPostById(this.ForumPostId);
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

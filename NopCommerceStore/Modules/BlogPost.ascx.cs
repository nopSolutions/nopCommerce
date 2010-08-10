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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Blog;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class BlogPostControl : BaseNopUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            this.BindData();
            base.OnInit(e);
        }
        
        private void BindData()
        {
            pnlError.Visible = false;

            var blogPost = BlogManager.GetBlogPostById(this.BlogPostId);
            if (blogPost != null)
            {
                this.lBlogPostTitle.Text = Server.HtmlEncode(blogPost.BlogPostTitle);
                this.lCreatedOn.Text = DateTimeHelper.ConvertToUserTime(blogPost.CreatedOn, DateTimeKind.Utc).ToString("D");
                this.lBlogPostBody.Text = blogPost.BlogPostBody;
                this.lTags.Text = RenderBlogTags(blogPost);

                if (blogPost.BlogPostAllowComments)
                {
                    if (!BlogManager.AllowNotRegisteredUsersToLeaveComments
                        && (NopContext.Current.User == null || NopContext.Current.User.IsGuest))
                    {
                        lblLeaveYourComment.Text = GetLocaleResourceString("Blog.OnlyRegisteredUsersCanLeaveComments");
                        txtComment.Enabled = false;
                        btnComment.Enabled = false;
                    }
                    else
                    {
                        lblLeaveYourComment.Text = GetLocaleResourceString("Blog.LeaveYourComment");
                        txtComment.Enabled = true;
                        btnComment.Enabled = true;
                    }

                    var blogComments = blogPost.BlogComments;
                    if (blogComments.Count > 0)
                    {
                        rptrComments.DataSource = blogComments;
                        rptrComments.DataBind();
                    }
                }
                else
                    pnlComments.Visible = false;
            }
            else
                Response.Redirect(CommonHelper.GetStoreLocation());
        }

        protected string RenderBlogTags(BlogPost blogPost)
        {
            StringBuilder sb = new StringBuilder();
            var tags = blogPost.ParsedTags;

            if (tags.Length > 0)
            {
                sb.Append(GetLocaleResourceString("Blog.Tags"));
                sb.Append(" ");

                for (int i = 0; i < tags.Length; i++)
                {
                    string tag = tags[i].Trim();
                    string url = SEOHelper.GetBlogUrlForTag(tag);
                    sb.Append(string.Format("<a href=\"{0}\">{1}</a>", url, Server.HtmlEncode(tag)));
                    if (i != tags.Length - 1)
                    {
                        sb.Append(", ");
                    }
                }
            }

            return sb.ToString();
        }
        
        protected void btnComment_Click(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid)
                {
                    var blogPost = BlogManager.GetBlogPostById(this.BlogPostId);
                    if (blogPost != null && blogPost.BlogPostAllowComments)
                    {
                        if (!BlogManager.AllowNotRegisteredUsersToLeaveComments
                            && (NopContext.Current.User == null || NopContext.Current.User.IsGuest))
                        {
                            lblLeaveYourComment.Text = GetLocaleResourceString("Blog.OnlyRegisteredUsersCanLeaveComments");
                            return;
                        }
                        string comment = txtComment.Text.Trim();
                        if (String.IsNullOrEmpty(comment))
                        {
                            throw new NopException(GetLocaleResourceString("Blog.PleaseEnterCommentText"));
                        }

                        int customerId = 0;
                        if (NopContext.Current.User != null && !NopContext.Current.User.IsGuest)
                            customerId = NopContext.Current.User.CustomerId;

                        BlogManager.InsertBlogComment(blogPost.BlogPostId, customerId, comment, DateTime.UtcNow);
                        txtComment.Text = string.Empty;
                        BindData();
                    }
                    else
                        Response.Redirect(CommonHelper.GetStoreLocation());
                }
            }
            catch (Exception exc)
            {
                pnlError.Visible = true;
                lErrorMessage.Text = Server.HtmlEncode(exc.Message);
            }
        }

        public int BlogPostId
        {
            get
            {
                return CommonHelper.QueryStringInt("BlogPostId");
            }
        }
    }
}

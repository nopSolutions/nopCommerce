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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Content.Blog;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class BlogCommentDetailsControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            BlogComment blogComment = BlogManager.GetBlogCommentById(this.BlogCommentId);
            if (blogComment != null)
            {
                this.lblCustomer.Text = GetCustomerInfo(blogComment.CustomerId);
                this.lblBlogPost.Text = GetBlogPostInfo(blogComment.BlogPostId);
                //this.txtComment.Value = blogComment.CommentText;
                this.lblIPAddress.Text = blogComment.IPAddress;
                this.txtComment.Text = blogComment.CommentText;
                this.lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(blogComment.CreatedOn, DateTimeKind.Utc).ToString();
            }
            else
                Response.Redirect("BlogComments.aspx");
        }

        protected string GetCustomerInfo(int customerId)
        {
            string customerInfo = string.Empty;
            Customer customer = CustomerManager.GetCustomerById(customerId);
            if (customer != null)
            {
                if (customer.IsGuest)
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, GetLocaleResourceString("Admin.BlogCommentDetails.Customer.Guest"));
                }
                else
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, Server.HtmlEncode(customer.Email));
                }
            }
            return customerInfo;
        }

        protected string GetBlogPostInfo(int blogPostId)
        {
            BlogPost blogPost = BlogManager.GetBlogPostById(blogPostId);
            if (blogPost != null)
            {
                string blogPostInfo = string.Format("<a href=\"BlogPostDetails.aspx?BlogPostID={0}\">{1}</a>", blogPost.BlogPostId, Server.HtmlEncode(blogPost.BlogPostTitle));
                return blogPostInfo;
            }
            else
                return string.Empty;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    BlogComment blogComment = BlogManager.GetBlogCommentById(this.BlogCommentId);
                    if (blogComment != null)
                    {
                        string comment = txtComment.Text;
                        blogComment = BlogManager.UpdateBlogComment(blogComment.BlogCommentId, blogComment.BlogPostId,
                            blogComment.CustomerId, blogComment.IPAddress, comment, blogComment.CreatedOn);
                        Response.Redirect("BlogCommentDetails.aspx?BlogCommentID=" + blogComment.BlogCommentId.ToString());
                    }
                    else
                        Response.Redirect("BlogComments.aspx");
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            BlogManager.DeleteBlogComment(this.BlogCommentId);
            Response.Redirect("BlogComments.aspx");
        }

        public int BlogCommentId
        {
            get
            {
                return CommonHelper.QueryStringInt("BlogCommentId");
            }
        }
    }
}
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
using System.Collections.Generic;
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
using NopSolutions.NopCommerce.Common.Utils;


namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class BlogCommentsControl : BaseNopAdministrationUserControl
    {
        protected void btnEditBlogComment_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "EditItem")
            {
                int blogCommentId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect("BlogCommentDetails.aspx?BlogCommentID=" + blogCommentId.ToString());
            }
        }

        protected void btnDeleteBlogComment_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "DeleteItem")
            {
                int blogCommentId = Convert.ToInt32(e.CommandArgument);
                BlogManager.DeleteBlogComment(blogCommentId);
                BindData();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                BindData();
        }

        protected void gvBlogComments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvBlogComments.PageIndex = e.NewPageIndex;
            BindData();
        }

        protected string GetCustomerInfo(int customerId)
        {
            string customerInfo = string.Empty;
            Customer customer = CustomerManager.GetCustomerById(customerId);
            if (customer != null)
            {
                if (customer.IsGuest)
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, GetLocaleResourceString("Admin.BlogComments.Customer.Guest"));
                }
                else
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, Server.HtmlEncode(customer.Email));
                }
            }
            return customerInfo;
        }

        protected void BindData()
        {
            List<BlogComment> blogComments = null;
            if (this.BlogPostId > 0)
                blogComments = BlogManager.GetBlogCommentsByBlogPostId(this.BlogPostId);
            else
                blogComments = BlogManager.GetAllBlogComments();

            gvBlogComments.DataSource = blogComments;
            gvBlogComments.DataBind();
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
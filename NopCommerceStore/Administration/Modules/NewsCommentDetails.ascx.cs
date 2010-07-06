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
using NopSolutions.NopCommerce.BusinessLogic.Content.NewsManagement;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class NewsCommentDetailsControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            NewsComment newsComment = NewsManager.GetNewsCommentById(this.NewsCommentId);
            if (newsComment != null)
            {
                this.txtTitle.Text = newsComment.Title;
                this.lblNews.Text = GetNewsInfo(newsComment.NewsId);
                this.lblCustomer.Text = GetCustomerInfo(newsComment.CustomerId);
                this.lblIPAddress.Text = newsComment.IPAddress;
                this.txtComment.Text = newsComment.Comment;
                this.lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(newsComment.CreatedOn, DateTimeKind.Utc).ToString();
            }
            else
                Response.Redirect("NewsComments.aspx");
        }

        protected string GetCustomerInfo(int customerId)
        {
            string customerInfo = string.Empty;
            Customer customer = CustomerManager.GetCustomerById(customerId);
            if (customer != null)
            {
                if (customer.IsGuest)
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, GetLocaleResourceString("Admin.NewsCommentDetails.Customer.Guest"));
                }
                else
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, Server.HtmlEncode(customer.Email));
                }
            }
            return customerInfo;
        }

        protected string GetNewsInfo(int newsId)
        {
            News news = NewsManager.GetNewsById(newsId);
            if (news != null)
            {
                string newsInfo = string.Format("<a href=\"NewsDetails.aspx?NewsID={0}\">{1}</a>", news.NewsId, Server.HtmlEncode(news.Title));
                return newsInfo;
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
                    NewsComment newsComment = NewsManager.GetNewsCommentById(this.NewsCommentId);
                    if (newsComment != null)
                    {
                        string title = txtTitle.Text.Trim();
                        string comment = txtComment.Text.Trim();
                        newsComment = NewsManager.UpdateNewsComment(newsComment.NewsCommentId, newsComment.NewsId,
                            newsComment.CustomerId, newsComment.IPAddress, title, comment, newsComment.CreatedOn);
                        Response.Redirect("NewsCommentDetails.aspx?NewsCommentID=" + newsComment.NewsCommentId.ToString());
                    }
                    else
                        Response.Redirect("NewsComments.aspx");
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            NewsManager.DeleteNewsComment(this.NewsCommentId);
            Response.Redirect("NewsComments.aspx");
        }

        public int NewsCommentId
        {
            get
            {
                return CommonHelper.QueryStringInt("NewsCommentId");
            }
        }
    }
}
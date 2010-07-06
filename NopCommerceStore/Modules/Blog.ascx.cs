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
using NopSolutions.NopCommerce.BusinessLogic.Content.Blog;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.SEO;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class BlogControl : BaseNopUserControl
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
            int totalRecords = 0;
            int pageSize = BlogManager.PostsPageSize;

            var blogPosts = BlogManager.GetAllBlogPosts(NopContext.Current.WorkingLanguage.LanguageId, pageSize, CurrentPageIndex, out totalRecords);
            if(blogPosts.Count > 0)
            {
                this.postsPager.PageSize = pageSize;
                this.postsPager.TotalRecords = totalRecords;
                this.postsPager.PageIndex = this.CurrentPageIndex;

                rptrBlogPosts.DataSource = blogPosts;
                rptrBlogPosts.DataBind();
            }
        }

        public int CurrentPageIndex
        {
            get
            {
                int _pageIndex = CommonHelper.QueryStringInt(postsPager.QueryStringProperty);
                _pageIndex--;
                if(_pageIndex < 0)
                    _pageIndex = 0;
                return _pageIndex;
            }
        }

        protected string GetBlogRSSUrl()
        {
            return SEOHelper.GetBlogRssUrl();
        }


        protected void rptrBlogPosts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var blogPost = e.Item.DataItem as BlogPost;
                var lComments = e.Item.FindControl("lComments") as Literal;
                lComments.Text = String.Format(GetLocaleResourceString("Blog.CommentsLink"), blogPost.BlogComments.Count);
            }
        }

    }
}

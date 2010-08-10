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
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class BlogControl : BaseNopUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            BindData();
            base.OnInit(e);
        }

        private void BindData()
        {
            if (String.IsNullOrEmpty(this.Tag))
            {
                int totalRecords = 0;
                int pageSize = BlogManager.PostsPageSize;
                DateTime? dateFrom = null;
                DateTime? dateTo = null;
                if (this.FilterByMonth.HasValue)
                {
                    dateFrom = this.FilterByMonth.Value;
                    dateTo = dateFrom.Value.AddMonths(1).AddSeconds(-1);

                    lTitle.Text = string.Format(GetLocaleResourceString("Blog.FilteredByMonth"), this.FilterByMonth.Value.Year, this.FilterByMonth.Value.ToString("MMMM"));
                }
                else
                {
                    lTitle.Text = GetLocaleResourceString("Blog.Blog");
                }

                var blogPosts = BlogManager.GetAllBlogPosts(NopContext.Current.WorkingLanguage.LanguageId,
                    dateFrom, dateTo, pageSize, CurrentPageIndex, out totalRecords);
                if (blogPosts.Count > 0)
                {
                    this.postsPager.PageSize = pageSize;
                    this.postsPager.TotalRecords = totalRecords;
                    this.postsPager.PageIndex = this.CurrentPageIndex;

                    rptrBlogPosts.DataSource = blogPosts;
                    rptrBlogPosts.DataBind();
                }
            }
            else
            {
                lTitle.Text = string.Format(GetLocaleResourceString("Blog.TaggedWith"), Server.HtmlEncode(this.Tag));
                var blogPosts = BlogManager.GetAllBlogPostsByTag(NopContext.Current.WorkingLanguage.LanguageId, this.Tag);
                if (blogPosts.Count > 0)
                {
                    rptrBlogPosts.DataSource = blogPosts;
                    rptrBlogPosts.DataBind();
                }
            }
        }

        protected string GetBlogRSSUrl()
        {
            return SEOHelper.GetBlogRssUrl();
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

        protected void rptrBlogPosts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var blogPost = e.Item.DataItem as BlogPost;
                var lComments = e.Item.FindControl("lComments") as Literal;
                lComments.Text = String.Format(GetLocaleResourceString("Blog.CommentsLink"), blogPost.BlogComments.Count);
            }
        }

        public int CurrentPageIndex
        {
            get
            {
                int _pageIndex = CommonHelper.QueryStringInt(postsPager.QueryStringProperty);
                _pageIndex--;
                if (_pageIndex < 0)
                    _pageIndex = 0;
                return _pageIndex;
            }
        }

        public string Tag
        {
            get
            {
                string tag = CommonHelper.QueryString("tag");
                return tag;
            }
        }

        public DateTime? FilterByMonth
        {
            get
            {
                DateTime? result = null;
                string dateStr = CommonHelper.QueryString("month");
                if (!String.IsNullOrEmpty(dateStr))
                {
                    string[] tempDate = dateStr.Split(new char[] { '-' });
                    if (tempDate.Length == 2)
                    {
                        result = new DateTime(Convert.ToInt32(tempDate[0]), Convert.ToInt32(tempDate[1]), 1);
                    }
                }
                return result;
            }
        }
    }
}

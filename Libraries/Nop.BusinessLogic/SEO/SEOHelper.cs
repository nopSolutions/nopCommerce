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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Blog;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.Content.NewsManagement;
using NopSolutions.NopCommerce.BusinessLogic.Content.Topics;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.SEO
{
    /// <summary>
    /// Represents a SEO helper
    /// </summary>
    public partial class SEOHelper
    {
        #region Methods
        /// <summary>
        /// Renders page meta tag
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="name">Meta name</param>
        /// <param name="content">Content</param>
        /// <param name="overwriteExisting">Overwrite existing content if exists</param>
        public static void RenderMetaTag(Page page, string name,
            string content, bool overwriteExisting)
        {
            if (page == null || page.Header == null)
                return;

            if (content == null)
                content = string.Empty;

            HtmlMeta control = page.Header.Controls.OfType<HtmlMeta>().FirstOrDefault(
                meta => string.Equals(meta.Name, name, StringComparison.OrdinalIgnoreCase));
            if (control == null)
            {
                control = new HtmlMeta();
                control.Name = name;
                control.Content = content;
                page.Header.Controls.Add(control);
            }
            else
            {
                if (overwriteExisting)
                    control.Content = content;
                else
                {
                    if (String.IsNullOrEmpty(control.Content))
                        control.Content = content;
                }
            }
        }

        /// <summary>
        /// Renders page title
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="title">Page title</param>
        /// <param name="overwriteExisting">Overwrite existing content if exists</param>
        public static void RenderTitle(Page page, string title, bool overwriteExisting)
        {
            bool includeStoreNameInTitle = SettingManager.GetSettingValueBoolean("SEO.IncludeStoreNameInTitle");
            RenderTitle(page, title, includeStoreNameInTitle, overwriteExisting);
        }

        /// <summary>
        /// Renders page title
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="title">Page title</param>
        /// <param name="includeStoreNameInTitle">Include store name in title</param>
        /// <param name="overwriteExisting">Overwrite existing content if exists</param>
        public static void RenderTitle(Page page, string title, 
            bool includeStoreNameInTitle, bool overwriteExisting)
        {
            if (page == null || page.Header == null)
                return;

            if (includeStoreNameInTitle)
                title = SettingManager.StoreName + ". " + title;

            if (String.IsNullOrEmpty(title))
                return;

            if (overwriteExisting)
                page.Title = HttpUtility.HtmlEncode(title);
            else
            {
                if (String.IsNullOrEmpty(page.Title))
                    page.Title = HttpUtility.HtmlEncode(title);
            }
        }

        /// <summary>
        /// Renders an RSS link to the page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="title">RSS Title</param>
        /// <param name="href">Path to the RSS feed</param>
        public static void RenderHeaderRssLink(Page page, string title, string href)
        {
            if (page == null || page.Header == null)
                return;

            var link = new HtmlGenericControl("link");
            link.Attributes.Add("href", href);
            link.Attributes.Add("type", "application/rss+xml");
            link.Attributes.Add("rel", "alternate");
            link.Attributes.Add("title", title);
            page.Header.Controls.Add(link);
        }

        /// <summary>
        /// Get SE name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Result</returns>
        public static string GetSEName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return string.Empty;
            string OKChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
            name = name.Trim().ToLowerInvariant();
            var sb = new StringBuilder();
            foreach (char c in name.ToCharArray())
                if (OKChars.Contains(c.ToString()))
                    sb.Append(c);
            string name2 = sb.ToString();
            name2 = name2.Replace(" ", "-");
            while (name2.Contains("--"))
                name2 = name2.Replace("--", "-");
            while (name2.Contains("__"))
                name2 = name2.Replace("__", "_");
            return HttpContext.Current.Server.UrlEncode(name2);
        }

        /// <summary>
        /// Gets access denied page URL for admin area
        /// </summary>
        /// <returns>Result URL</returns>
        public static string GetAdminAreaAccessDeniedUrl()
        {
            string url = CommonHelper.GetStoreAdminLocation() + "AccessDenied.aspx";
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets login page URL
        /// </summary>
        /// <returns>Login page URL</returns>
        public static string GetLoginPageUrl()
        {
            return GetLoginPageUrl(string.Empty);
        }

        /// <summary>
        /// Gets login page URL
        /// </summary>
        /// <param name="returnUrl">Return url</param>
        /// <returns>Login page URL</returns>
        public static string GetLoginPageUrl(string returnUrl)
        {
            string loginUrl = string.Empty;
            if (!string.IsNullOrEmpty(returnUrl))
            {
                loginUrl = string.Format(CultureInfo.InvariantCulture, "{0}Login.aspx?ReturnUrl={1}",
                    CommonHelper.GetStoreLocation(),
                    HttpUtility.UrlEncode(returnUrl));
            }
            else
            {
                loginUrl = string.Format(CultureInfo.InvariantCulture, "{0}Login.aspx",
                    CommonHelper.GetStoreLocation());
            }
            return loginUrl.ToLowerInvariant();
        }

        /// <summary>
        /// Gets login page URL
        /// </summary>
        /// <param name="addCurrentPageUrl">A value indicating whether add current page url as "ReturnURL" parameter</param>
        /// <returns>Login page URL</returns>
        public static string GetLoginPageUrl(bool addCurrentPageUrl)
        {
            return GetLoginPageUrl(addCurrentPageUrl, false);
        }

        /// <summary>
        /// Gets login page URL
        /// </summary>
        /// <param name="addCurrentPageUrl">A value indicating whether add current page url as "ReturnURL" parameter</param>
        /// <param name="checkoutAsGuest">A value indicating whether login page will show "Checkout as a guest or Register" message</param>
        /// <returns>Login page URL</returns>
        public static string GetLoginPageUrl(bool addCurrentPageUrl, bool checkoutAsGuest)
        {
            string loginUrl = string.Empty;
            if (addCurrentPageUrl)
            {
                string rawUrl = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                    rawUrl = HttpContext.Current.Request.RawUrl;
                loginUrl = string.Format(CultureInfo.InvariantCulture, "{0}Login.aspx?ReturnUrl={1}",
                    CommonHelper.GetStoreLocation(),
                    HttpUtility.UrlEncode(rawUrl));
            }
            else
            {
                loginUrl = GetLoginPageUrl();
            }

            if (checkoutAsGuest)
            {
                loginUrl = CommonHelper.ModifyQueryString(loginUrl, "CheckoutAsGuest=true", string.Empty);
            }
            return loginUrl.ToLowerInvariant();
        }

        /// <summary>
        /// Gets login page URL of admin area
        /// </summary>
        /// <returns>Login page URL</returns>
        public static string GetAdminAreaLoginPageUrl()
        {
            string url = string.Format(CultureInfo.InvariantCulture, "{0}Login.aspx",
                      CommonHelper.GetStoreAdminLocation());
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets product URL
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product URL</returns>
        public static string GetProductUrl(int productId)
        {
            var product = ProductManager.GetProductById(productId);
            return GetProductUrl(product);
        }

        /// <summary>
        /// Gets product URL
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Product URL</returns>
        public static string GetProductUrl(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");
            string seName = GetSEName(product.SEName);
            if (String.IsNullOrEmpty(seName))
            {
                seName = GetSEName(product.Name);
            }
            string url = string.Format(SettingManager.GetSettingValue("SEO.Product.UrlRewriteFormat"), 
                CommonHelper.GetStoreLocation(), product.ProductId, seName);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets product email a friend URL
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product email a friend URL</returns>
        public static string GetProductEmailAFriendUrl(int productId)
        {
            string url = string.Format("{0}ProductEmailAFriend.aspx?ProductId={1}", CommonHelper.GetStoreLocation(), productId);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets manufacturer URL
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>Manufacturer URL</returns>
        public static string GetManufacturerUrl(int manufacturerId)
        {
            var manufacturer = ManufacturerManager.GetManufacturerById(manufacturerId);
            return GetManufacturerUrl(manufacturer);
        }

        /// <summary>
        /// Gets manufacturer URL
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        /// <returns>Manufacturer URL</returns>
        public static string GetManufacturerUrl(Manufacturer manufacturer)
        {
            if (manufacturer == null)
                throw new ArgumentNullException("manufacturer");
            string seName = GetSEName(manufacturer.SEName);
            if (String.IsNullOrEmpty(seName))
            {
                seName = GetSEName(manufacturer.Name);
            } 
            string url = string.Format(SettingManager.GetSettingValue("SEO.Manufacturer.UrlRewriteFormat"), 
                CommonHelper.GetStoreLocation(), manufacturer.ManufacturerId, seName);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets category URL
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category URL</returns>
        public static string GetCategoryUrl(int categoryId)
        {
            var category = CategoryManager.GetCategoryById(categoryId);
            return GetCategoryUrl(category);
        }

        /// <summary>
        /// Gets category URL
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Category URL</returns>
        public static string GetCategoryUrl(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category"); 
            string seName = GetSEName(category.SEName);
            if (String.IsNullOrEmpty(seName))
            {
                seName = GetSEName(category.Name);
            }
            string url = string.Format(SettingManager.GetSettingValue("SEO.Category.UrlRewriteFormat"), CommonHelper.GetStoreLocation(), category.CategoryId, seName);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets blog post URL
        /// </summary>
        /// <param name="blogPostId">Blog post identifier</param>
        /// <returns>Blog post URL</returns>
        public static string GetBlogPostUrl(int blogPostId)
        {
            var blogPost = BlogManager.GetBlogPostById(blogPostId);
            return GetBlogPostUrl(blogPost);
        }

        /// <summary>
        /// Gets blog post URL
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        /// <returns>Blog post URL</returns>
        public static string GetBlogPostUrl(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException("blogPost"); 
            string seName = GetSEName(blogPost.BlogPostTitle);
            string url = string.Format(SettingManager.GetSettingValue("SEO.Blog.UrlRewriteFormat"), 
                CommonHelper.GetStoreLocation(), blogPost.BlogPostId, seName);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets news URL
        /// </summary>
        /// <param name="newsId">News identifier</param>
        /// <returns>News URL</returns>
        public static string GetNewsUrl(int newsId)
        {
            var news = NewsManager.GetNewsById(newsId);
            return GetNewsUrl(news);
        }

        /// <summary>
        /// Gets news URL
        /// </summary>
        /// <param name="news">News item</param>
        /// <returns>News URL</returns>
        public static string GetNewsUrl(News news)
        {
            if (news == null)
                throw new ArgumentNullException("news"); 
            string seName = GetSEName(news.Title);
            string url = string.Format(SettingManager.GetSettingValue("SEO.News.UrlRewriteFormat"), 
                CommonHelper.GetStoreLocation(), news.NewsId, seName);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets news Rss URL
        /// </summary>
        /// <returns>News Rss URL</returns>
        public static string GetNewsRssUrl()
        {
            return GetNewsRssUrl(NopContext.Current.WorkingLanguage.LanguageId);
        }

        /// <summary>
        /// Gets news Rss URL
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>News Rss URL</returns>
        public static string GetNewsRssUrl(int languageId)
        {
            string url = CommonHelper.GetStoreLocation() + "NewsRSS.aspx?LanguageId=" + languageId.ToString();
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets blog Rss URL
        /// </summary>
        /// <returns>Blog Rss URL</returns>
        public static string GetBlogRssUrl()
        {
            return GetBlogRssUrl(NopContext.Current.WorkingLanguage.LanguageId);
        }

        /// <summary>
        /// Gets blog Rss URL
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Blog Rss URL</returns>
        public static string GetBlogRssUrl(int languageId)
        {
            string url = CommonHelper.GetStoreLocation() + "BlogRSS.aspx?LanguageId=" + languageId.ToString();
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets forum URL
        /// </summary>
        /// <returns>Forum URL</returns>
        public static string GetForumMainUrl()
        {
            string url = string.Format("{0}Boards/", CommonHelper.GetStoreLocation());
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets forum URL
        /// </summary>
        /// <returns>Forum URL</returns>
        public static string GetForumActiveDiscussionsUrl()
        {
            string url = string.Format("{0}Boards/ActiveDiscussions.aspx", CommonHelper.GetStoreLocation());
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets forum group URL
        /// </summary>
        /// <param name="forumGroupId">Forum group identifier</param>
        /// <returns>Forum group URL</returns>
        public static string GetForumGroupUrl(int forumGroupId)
        {
            var forumGroup = ForumManager.GetForumGroupById(forumGroupId);
            return GetForumGroupUrl(forumGroup);
        }

        /// <summary>
        /// Gets forum group URL
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        /// <returns>Forum group URL</returns>
        public static string GetForumGroupUrl(ForumGroup forumGroup)
        {
            if(forumGroup == null)
            {
                throw new ArgumentNullException("forumGroup");
            }
            string seName = GetSEName(forumGroup.Name);
            string url = string.Format(SettingManager.GetSettingValue("SEO.ForumGroup.UrlRewriteFormat"), 
                CommonHelper.GetStoreLocation(), forumGroup.ForumGroupId, seName);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets forum URL
        /// </summary>
        /// <param name="forumId">Forum identifier</param>
        /// <returns>Forum URL</returns>
        public static string GetForumUrl(int forumId)
        {
            var forum = ForumManager.GetForumById(forumId);
            return GetForumUrl(forum);
        }

        /// <summary>
        /// Gets forum URL
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>Forum URL</returns>
        public static string GetForumUrl(Forum forum)
        {
            if(forum == null)
            {
                throw new ArgumentNullException("Forum");
            }
            string seName = GetSEName(forum.Name);
            string url = string.Format(SettingManager.GetSettingValue("SEO.Forum.UrlRewriteFormat"), 
                CommonHelper.GetStoreLocation(), forum.ForumId, seName);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets move topic URL
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>Forum URL</returns>
        public static string GetMoveForumTopicUrl(ForumTopic forumTopic)
        {
            if (forumTopic == null)
                throw new ArgumentNullException("forumTopic");

            string url = string.Format("{0}Boards/MoveTopic.aspx?TopicId={1}", CommonHelper.GetStoreLocation(), forumTopic.ForumTopicId);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets forum search URL
        /// </summary>
        /// <param name="searchTerms">Search terms</param>
        /// <returns>Forum URL</returns>
        public static string GetForumSearchUrl(string searchTerms)
        {
            string url = string.Format("{0}Boards/Search.aspx?searchTerms={1}", CommonHelper.GetStoreLocation(), HttpUtility.UrlEncode(searchTerms));
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets forum topic URL
        /// </summary>
        /// <param name="topicId">Forum topic identifier</param>
        /// <returns>Forum topic URL</returns>
        public static string GetForumTopicUrl(int topicId)
        {
            var topic = ForumManager.GetTopicById(topicId);
            return GetForumTopicUrl(topic); 
        }

        /// <summary>
        /// Gets forum topic URL
        /// </summary>
        /// <param name="topic">Forum topic</param>
        /// <returns>Forum topic URL</returns>
        public static string GetForumTopicUrl(ForumTopic topic)
        {
            return GetForumTopicUrl(topic, "p", null);
        }

        /// <summary>
        /// Gets forum topic URL
        /// </summary>
        /// <param name="topicId">Forum topic identifier</param>
        /// <param name="queryStringProperty">Query string property</param>
        /// <param name="pageIndex">Page index</param>
        /// <returns>Forum topic URL</returns>
        public static string GetForumTopicUrl(int topicId, 
            string queryStringProperty, int? pageIndex)
        {
            var topic = ForumManager.GetTopicById(topicId);
            return GetForumTopicUrl(topic, queryStringProperty, pageIndex);
        }

        /// <summary>
        /// Gets forum topic URL
        /// </summary>
        /// <param name="topic">Forum topic</param>
        /// <param name="queryStringProperty">Query string property</param>
        /// <param name="pageIndex">Page index</param>
        /// <returns>Forum topic URL</returns>
        public static string GetForumTopicUrl(ForumTopic topic,
            string queryStringProperty, int? pageIndex)
        {
            return GetForumTopicUrl(topic, queryStringProperty, pageIndex, null);
        }

        /// <summary>
        /// Gets forum topic URL
        /// </summary>
        /// <param name="topicId">Forum topic identifier</param>
        /// <param name="queryStringProperty">Query string property</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="postId">Post identifier (anchor)</param>
        /// <returns>Forum topic URL</returns>
        public static string GetForumTopicUrl(int topicId, string queryStringProperty, 
            int? pageIndex, int? postId)
        {
            var topic = ForumManager.GetTopicById(topicId);
            return GetForumTopicUrl(topic, queryStringProperty, pageIndex, postId);
        }

        /// <summary>
        /// Gets forum topic URL
        /// </summary>
        /// <param name="topic">Forum topic</param>
        /// <param name="queryStringProperty">Query string property</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="postId">Post identifier (anchor)</param>
        /// <returns>Forum topic URL</returns>
        public static string GetForumTopicUrl(ForumTopic topic, string queryStringProperty,
            int? pageIndex, int? postId)
        {
            if(topic == null)
            {
                throw new ArgumentNullException("forumTopic");
            }
            string seName = GetSEName(topic.Subject);
            string url = string.Empty;
            url = string.Format(SettingManager.GetSettingValue("SEO.ForumTopic.UrlRewriteFormat"), CommonHelper.GetStoreLocation(), topic.ForumTopicId, seName);
            if(pageIndex.HasValue && pageIndex.Value > 1)
            {
                url += string.Format("?{0}={1}", queryStringProperty, pageIndex.Value);
            }
            if(postId.HasValue)
            {
                url += string.Format("#{0}", postId.Value);
            }
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets new forum topic URL
        /// </summary>
        /// <param name="forumId">Forum identifier</param>
        /// <returns>New forum topic URL</returns>
        public static string GetNewForumTopicUrl(int forumId)
        {
            string url = string.Format("{0}Boards/TopicNew.aspx?ForumId={1}", CommonHelper.GetStoreLocation(), forumId);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets edit topic URL
        /// </summary>
        /// <param name="topicId">Forum post identifier</param>
        /// <returns>Edit forum post URL</returns>
        public static string GetEditForumTopicUrl(int topicId)
        {
            string url = string.Format("{0}Boards/TopicEdit.aspx?TopicId={1}", CommonHelper.GetStoreLocation(), topicId);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets new forum post URL
        /// </summary>
        /// <param name="topicId">Forum topic identifier</param>
        /// <returns>New forum post URL</returns>
        public static string GetNewForumPostUrl(int topicId)
        {
            string url = string.Format("{0}Boards/PostNew.aspx?TopicId={1}", CommonHelper.GetStoreLocation(), topicId);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets new forum post URL with quoting post
        /// </summary>
        /// <param name="topicId">Forum topic identifier</param>
        /// <param name="quotePostId">Quoting post identifier</param>
        /// <returns>New forum post URL</returns>
        public static string GetNewForumPostUrl(int topicId, int quotePostId)
        {
            string url = string.Format("{0}Boards/PostNew.aspx?TopicId={1}&QuotePostId={2}", CommonHelper.GetStoreLocation(), topicId, quotePostId);
            return url.ToLowerInvariant();
        }
        
        /// <summary>
        /// Gets edit post URL
        /// </summary>
        /// <param name="forumPostId">Forum post identifier</param>
        /// <returns>Edit forum post URL</returns>
        public static string GetEditForumPostUrl(int forumPostId)
        {
            string url = string.Format("{0}Boards/PostEdit.aspx?PostId={1}", CommonHelper.GetStoreLocation(), forumPostId);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets forum user profile URL
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>Forum topic URL</returns>
        public static string GetUserProfileUrl(int userId)
        {
            string url = string.Format("{0}Profile.aspx?UserId={1}", CommonHelper.GetStoreLocation(), userId);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets Topic page URL
        /// </summary>
        /// <param name="topicId">Topic identifier</param>
        /// <param name="title">Localized topic title</param>
        /// <returns>Topic page URL</returns>
        public static string GetTopicUrl(int topicId, string title)
        {
            string url = string.Format(SettingManager.GetSettingValue("SEO.Topic.UrlRewriteFormat"), CommonHelper.GetStoreLocation(), topicId, GetSEName(title));
            return url.ToLowerInvariant();
        }

        #endregion
    }
}

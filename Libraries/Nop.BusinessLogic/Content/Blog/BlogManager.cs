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
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Utils.Html;

namespace NopSolutions.NopCommerce.BusinessLogic.Content.Blog
{
    /// <summary>
    /// Blog post manager
    /// </summary>
    public partial class BlogManager
    {
        #region Constants
        private const string BLOGPOST_BY_ID_KEY = "Nop.blogpost.id-{0}";
        private const string BLOGPOST_PATTERN_KEY = "Nop.blogpost.";
        #endregion
        
        #region Methods
        /// <summary>
        /// Deletes an blog post
        /// </summary>
        /// <param name="blogPostId">Blog post identifier</param>
        public static void DeleteBlogPost(int blogPostId)
        {
            var blogPost = GetBlogPostById(blogPostId);
            if (blogPost == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(blogPost))
                context.BlogPosts.Attach(blogPost);
            context.DeleteObject(blogPost);
            context.SaveChanges();
            
            if (BlogManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(BLOGPOST_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets an blog post
        /// </summary>
        /// <param name="blogPostId">Blog post identifier</param>
        /// <returns>Blog post</returns>
        public static BlogPost GetBlogPostById(int blogPostId)
        {
            if (blogPostId == 0)
                return null;

            string key = string.Format(BLOGPOST_BY_ID_KEY, blogPostId);
            object obj2 = NopRequestCache.Get(key);
            if (BlogManager.CacheEnabled && (obj2 != null))
            {
                return (BlogPost)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from bp in context.BlogPosts
                        where bp.BlogPostId == blogPostId
                        select bp;
            var blogPost = query.SingleOrDefault();

            if (BlogManager.CacheEnabled)
            {
                NopRequestCache.Add(key, blogPost);
            }
            return blogPost;
        }

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <returns>Blog posts</returns>
        public static List<BlogPost> GetAllBlogPosts(int languageId)
        {
            int totalRecords;
            return GetAllBlogPosts(languageId, Int32.MaxValue, 0, out totalRecords);
        }

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Blog posts</returns>
        public static List<BlogPost> GetAllBlogPosts(int languageId, int pageSize,
            int pageIndex, out int totalRecords)
        {
            if(pageSize <= 0)
                pageSize = 10;
            if(pageSize == Int32.MaxValue)
                pageSize = Int32.MaxValue - 1;
            if(pageIndex < 0)
                pageIndex = 0;
            if(pageIndex == Int32.MaxValue)
                pageIndex = Int32.MaxValue - 1;

            var context = ObjectContextHelper.CurrentObjectContext;
            var blogPosts = context.Sp_BlogPostLoadAll(languageId,
                pageSize, pageIndex, out totalRecords).ToList();

            return blogPosts;
        }

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="tag">Tag</param>
        /// <returns>Blog posts</returns>
        public static List<BlogPost> GetAllBlogPostsByTag(int languageId, string tag)
        {
            tag = tag.Trim();

            var blogPostsAll = GetAllBlogPosts(languageId);
            List<BlogPost> blogPosts = new List<BlogPost>();
            foreach (var blogPost in blogPostsAll)
            {
                var tags = blogPost.ParsedTags;
                if (!String.IsNullOrEmpty(tags.FirstOrDefault(t => t.Equals(tag, StringComparison.InvariantCultureIgnoreCase))))
                {
                    blogPosts.Add(blogPost);
                }
            }

            return blogPosts;
        }

        /// <summary>
        /// Inserts an blog post
        /// </summary>
        /// <param name="languageId">The language identifier</param>
        /// <param name="blogPostTitle">The blog post title</param>
        /// <param name="blogPostBody">The blog post title</param>
        /// <param name="blogPostAllowComments">A value indicating whether the blog post comments are allowed</param>
        /// <param name="tags">The blog post tags</param>
        /// <param name="createdById">The user identifier who created the blog post</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <returns>Blog post</returns>
        public static BlogPost InsertBlogPost(int languageId, string blogPostTitle,
            string blogPostBody, bool blogPostAllowComments,
            string tags, int createdById, DateTime createdOn)
        {
            blogPostTitle = CommonHelper.EnsureMaximumLength(blogPostTitle, 200);
            tags = CommonHelper.EnsureMaximumLength(tags, 4000);

            var context = ObjectContextHelper.CurrentObjectContext;

            var blogPost = context.BlogPosts.CreateObject();
            blogPost.LanguageId = languageId;
            blogPost.BlogPostTitle = blogPostTitle;
            blogPost.BlogPostBody = blogPostBody;
            blogPost.BlogPostAllowComments = blogPostAllowComments;
            blogPost.Tags = tags;
            blogPost.CreatedById = createdById;
            blogPost.CreatedOn = createdOn;

            context.BlogPosts.AddObject(blogPost);
            context.SaveChanges();

            if (BlogManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(BLOGPOST_PATTERN_KEY);
            }

            return blogPost;
        }

        /// <summary>
        /// Updates the blog post
        /// </summary>
        /// <param name="blogPostId">The blog post identifier</param>
        /// <param name="languageId">The language identifier</param>
        /// <param name="blogPostTitle">The blog post title</param>
        /// <param name="blogPostBody">The blog post title</param>
        /// <param name="blogPostAllowComments">A value indicating whether the blog post comments are allowed</param>
        /// <param name="tags">The blog post tags</param>
        /// <param name="createdById">The user identifier who created the blog post</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <returns>Blog post</returns>
        public static BlogPost UpdateBlogPost(int blogPostId,
            int languageId, string blogPostTitle,
            string blogPostBody, bool blogPostAllowComments,
            string tags, int createdById, DateTime createdOn)
        {
            blogPostTitle = CommonHelper.EnsureMaximumLength(blogPostTitle, 200);
            tags = CommonHelper.EnsureMaximumLength(tags, 4000);

            var blogPost = GetBlogPostById(blogPostId);
            if (blogPost == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(blogPost))
                context.BlogPosts.Attach(blogPost);

            blogPost.LanguageId = languageId;
            blogPost.BlogPostTitle = blogPostTitle;
            blogPost.BlogPostBody = blogPostBody;
            blogPost.BlogPostAllowComments = blogPostAllowComments;
            blogPost.Tags = tags;
            blogPost.CreatedById = createdById;
            blogPost.CreatedOn = createdOn;
            context.SaveChanges();

            if (BlogManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(BLOGPOST_PATTERN_KEY);
            }

            return blogPost;
        }

        /// <summary>
        /// Deletes an blog comment
        /// </summary>
        /// <param name="blogCommentId">Blog comment identifier</param>
        public static void DeleteBlogComment(int blogCommentId)
        {
            var blogComment = GetBlogCommentById(blogCommentId);
            if (blogComment == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(blogComment))
                context.BlogComments.Attach(blogComment);
            context.DeleteObject(blogComment);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets an blog comment
        /// </summary>
        /// <param name="blogCommentId">Blog comment identifier</param>
        /// <returns>A blog comment</returns>
        public static BlogComment GetBlogCommentById(int blogCommentId)
        {
            if (blogCommentId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from bc in context.BlogComments
                        where bc.BlogCommentId == blogCommentId
                        select bc;
            var blogComment = query.SingleOrDefault();
            return blogComment;
        }

        /// <summary>
        /// Gets a collection of blog comments by blog post identifier
        /// </summary>
        /// <param name="blogPostId">Blog post identifier</param>
        /// <returns>A collection of blog comments</returns>
        public static List<BlogComment> GetBlogCommentsByBlogPostId(int blogPostId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from bc in context.BlogComments
                        orderby bc.CreatedOn
                        where bc.BlogPostId == blogPostId
                        select bc;
            var collection = query.ToList();
            return collection;
        }

        /// <summary>
        /// Gets all blog comments
        /// </summary>
        /// <returns>Blog comments</returns>
        public static List<BlogComment> GetAllBlogComments()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from bc in context.BlogComments
                        orderby bc.CreatedOn
                        select bc;
            var collection = query.ToList();
            return collection;
        }

        /// <summary>
        /// Inserts a blog comment
        /// </summary>
        /// <param name="blogPostId">The blog post identifier</param>
        /// <param name="customerId">The customer identifier who commented the blog post</param>
        /// <param name="commentText">The comment text</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <returns>Blog comment</returns>
        public static BlogComment InsertBlogComment(int blogPostId,
            int customerId, string commentText, DateTime createdOn)
        {
            return InsertBlogComment(blogPostId, customerId, commentText,
                createdOn, BlogManager.NotifyAboutNewBlogComments);
        }

        /// <summary>
        /// Inserts a blog comment
        /// </summary>
        /// <param name="blogPostId">The blog post identifier</param>
        /// <param name="customerId">The customer identifier who commented the blog post</param>
        /// <param name="commentText">The comment text</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="notify">A value indicating whether to notify the store owner</param>
        /// <returns>Blog comment</returns>
        public static BlogComment InsertBlogComment(int blogPostId,
            int customerId, string commentText, DateTime createdOn, bool notify)
        {
            string IPAddress = NopContext.Current.UserHostAddress;
            return InsertBlogComment(blogPostId, customerId, IPAddress, commentText, createdOn, notify);
        }

        /// <summary>
        /// Inserts a blog comment
        /// </summary>
        /// <param name="blogPostId">The blog post identifier</param>
        /// <param name="customerId">The customer identifier who commented the blog post</param>
        /// <param name="ipAddress">The IP address</param>
        /// <param name="commentText">The comment text</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="notify">A value indicating whether to notify the store owner</param>
        /// <returns>Blog comment</returns>
        public static BlogComment InsertBlogComment(int blogPostId,
            int customerId, string ipAddress, string commentText, DateTime createdOn, bool notify)
        {
            ipAddress = CommonHelper.EnsureMaximumLength(ipAddress, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            var blogComment = context.BlogComments.CreateObject();
            blogComment.BlogPostId = blogPostId;
            blogComment.CustomerId = customerId;
            blogComment.IPAddress = ipAddress;
            blogComment.CommentText = commentText;
            blogComment.CreatedOn = createdOn;

            context.BlogComments.AddObject(blogComment);
            context.SaveChanges();

            if (notify)
            {
                MessageManager.SendBlogCommentNotificationMessage(blogComment, LocalizationManager.DefaultAdminLanguage.LanguageId);
            }

            return blogComment;
        }

        /// <summary>
        /// Updates the blog comment
        /// </summary>
        /// <param name="blogCommentId">The blog comment identifier</param>
        /// <param name="blogPostId">The blog post identifier</param>
        /// <param name="customerId">The customer identifier who commented the blog post</param>
        /// <param name="ipAddress">The IP address</param>
        /// <param name="commentText">The comment text</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <returns>Blog comment</returns>
        public static BlogComment UpdateBlogComment(int blogCommentId, int blogPostId,
            int customerId, string ipAddress, string commentText, DateTime createdOn)
        {
            ipAddress = CommonHelper.EnsureMaximumLength(ipAddress, 100);

            var blogComment = GetBlogCommentById(blogCommentId);
            if (blogComment == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(blogComment))
                context.BlogComments.Attach(blogComment);

            blogComment.BlogPostId = blogPostId;
            blogComment.CustomerId = customerId;
            blogComment.IPAddress = ipAddress;
            blogComment.CommentText = commentText;
            blogComment.CreatedOn = createdOn;
            context.SaveChanges();
            return blogComment;
        }
        
        /// <summary>
        /// Formats the text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string FormatCommentText(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, false, false, false);
            return text;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public static bool CacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.BlogManager.CacheEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether blog is enabled
        /// </summary>
        public static bool BlogEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Common.EnableBlog");
            }
            set
            {
                SettingManager.SetParam("Common.EnableBlog", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the page size for posts
        /// </summary>
        public static int PostsPageSize
        {
            get
            {
                return SettingManager.GetSettingValueInteger("Blog.PostsPageSize", 10);
            }
            set
            {
                SettingManager.SetParam("Blog.PostsPageSize", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether not registered user can leave comments
        /// </summary>
        public static bool AllowNotRegisteredUsersToLeaveComments
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Blog.AllowNotRegisteredUsersToLeaveComments");
            }
            set
            {
                SettingManager.SetParam("Blog.AllowNotRegisteredUsersToLeaveComments", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to notify about new blog comments
        /// </summary>
        public static bool NotifyAboutNewBlogComments
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Blog.NotifyAboutNewBlogComments");
            }
            set
            {
                SettingManager.SetParam("Blog.NotifyAboutNewBlogComments", value.ToString());
            }
        }
        #endregion
    }
}

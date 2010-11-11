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
using System.Data.Objects;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Utils.Html;

namespace NopSolutions.NopCommerce.BusinessLogic.Content.Blog
{
    /// <summary>
    /// Blog post service
    /// </summary>
    public partial class BlogService : IBlogService
    {
        #region Constants
        private const string BLOGPOST_BY_ID_KEY = "Nop.blogpost.id-{0}";
        private const string BLOGPOST_PATTERN_KEY = "Nop.blogpost.";
        #endregion

        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        protected NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        protected ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public BlogService(NopObjectContext context)
        {
            _context = context;
            _cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Deletes an blog post
        /// </summary>
        /// <param name="blogPostId">Blog post identifier</param>
        public void DeleteBlogPost(int blogPostId)
        {
            var blogPost = GetBlogPostById(blogPostId);
            if (blogPost == null)
                return;

            
            if (!_context.IsAttached(blogPost))
                _context.BlogPosts.Attach(blogPost);
            _context.DeleteObject(blogPost);
            _context.SaveChanges();
            
            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(BLOGPOST_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets an blog post
        /// </summary>
        /// <param name="blogPostId">Blog post identifier</param>
        /// <returns>Blog post</returns>
        public BlogPost GetBlogPostById(int blogPostId)
        {
            if (blogPostId == 0)
                return null;

            string key = string.Format(BLOGPOST_BY_ID_KEY, blogPostId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (BlogPost)obj2;
            }

            
            var query = from bp in _context.BlogPosts
                        where bp.BlogPostId == blogPostId
                        select bp;
            var blogPost = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, blogPost);
            }
            return blogPost;
        }

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all records</param>
        /// <returns>Blog posts</returns>
        public List<BlogPost> GetAllBlogPosts(int languageId)
        {
            return GetAllBlogPosts(languageId, 0, int.MaxValue);
        }

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Blog posts</returns>
        public PagedList<BlogPost> GetAllBlogPosts(int languageId, int pageIndex, int pageSize)
        {
            return GetAllBlogPosts(languageId,
                null, null, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="dateFrom">Filter by created date; null if you want to get all records</param>
        /// <param name="dateTo">Filter by created date; null if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Blog posts</returns>
        public PagedList<BlogPost> GetAllBlogPosts(int languageId,
            DateTime? dateFrom, DateTime? dateTo, int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
                pageIndex = 0;
            if (pageIndex == Int32.MaxValue)
                pageIndex = Int32.MaxValue - 1;
            if (pageSize <= 0)
                pageSize = 10;
            if (pageSize == Int32.MaxValue)
                pageSize = Int32.MaxValue - 1;

            
            var query = from bp in _context.BlogPosts
                        where (!dateFrom.HasValue || dateFrom.Value <= bp.CreatedOn) &&
                        (!dateTo.HasValue || dateTo.Value >= bp.CreatedOn) &&
                        (languageId == 0 || languageId == bp.LanguageId)
                        orderby bp.CreatedOn descending
                        select bp;
            var blogPosts = new PagedList<BlogPost>(query, pageIndex, pageSize);
            return blogPosts;
        }

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="tag">Tag</param>
        /// <returns>Blog posts</returns>
        public List<BlogPost> GetAllBlogPostsByTag(int languageId, string tag)
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
        /// Gets all blog post tags
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <returns>Blog post tags</returns>
        public List<BlogPostTag> GetAllBlogPostTags(int languageId)
        {
            List<BlogPostTag> blogPostTags = new List<BlogPostTag>();

            var blogPostsAll = GetAllBlogPosts(languageId);
            foreach (var blogPost in blogPostsAll)
            {
                var tags = blogPost.ParsedTags;
                foreach (string tag in tags)
                {
                    var foundBlogPostTag = blogPostTags.Find(bpt => bpt.Name.Equals(tag, StringComparison.InvariantCultureIgnoreCase));
                    if (foundBlogPostTag == null)
                    {
                        foundBlogPostTag = new BlogPostTag()
                        {
                            Name = tag,
                            BlogPostCount = 1
                        };
                        blogPostTags.Add(foundBlogPostTag);
                    }
                    else
                    {
                        foundBlogPostTag.BlogPostCount++;
                    }
                }
            }

            return blogPostTags;
        }

        /// <summary>
        /// Inserts an blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        public void InsertBlogPost(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException("blogPost");

            blogPost.BlogPostTitle = CommonHelper.EnsureNotNull(blogPost.BlogPostTitle);
            blogPost.BlogPostTitle = CommonHelper.EnsureMaximumLength(blogPost.BlogPostTitle, 200);
            blogPost.BlogPostBody = CommonHelper.EnsureNotNull(blogPost.BlogPostBody);
            blogPost.Tags = CommonHelper.EnsureNotNull(blogPost.Tags);
            blogPost.Tags = CommonHelper.EnsureMaximumLength(blogPost.Tags, 4000);

            

            _context.BlogPosts.AddObject(blogPost);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(BLOGPOST_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        public void UpdateBlogPost(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException("blogPost");

            blogPost.BlogPostTitle = CommonHelper.EnsureNotNull(blogPost.BlogPostTitle);
            blogPost.BlogPostTitle = CommonHelper.EnsureMaximumLength(blogPost.BlogPostTitle, 200);
            blogPost.BlogPostBody = CommonHelper.EnsureNotNull(blogPost.BlogPostBody);
            blogPost.Tags = CommonHelper.EnsureNotNull(blogPost.Tags);
            blogPost.Tags = CommonHelper.EnsureMaximumLength(blogPost.Tags, 4000);

            
            if (!_context.IsAttached(blogPost))
                _context.BlogPosts.Attach(blogPost);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(BLOGPOST_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Deletes an blog comment
        /// </summary>
        /// <param name="blogCommentId">Blog comment identifier</param>
        public void DeleteBlogComment(int blogCommentId)
        {
            var blogComment = GetBlogCommentById(blogCommentId);
            if (blogComment == null)
                return;

            
            if (!_context.IsAttached(blogComment))
                _context.BlogComments.Attach(blogComment);
            _context.DeleteObject(blogComment);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets an blog comment
        /// </summary>
        /// <param name="blogCommentId">Blog comment identifier</param>
        /// <returns>A blog comment</returns>
        public BlogComment GetBlogCommentById(int blogCommentId)
        {
            if (blogCommentId == 0)
                return null;

            
            var query = from bc in _context.BlogComments
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
        public List<BlogComment> GetBlogCommentsByBlogPostId(int blogPostId)
        {
            
            var query = from bc in _context.BlogComments
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
        public List<BlogComment> GetAllBlogComments()
        {
            
            var query = from bc in _context.BlogComments
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
        public BlogComment InsertBlogComment(int blogPostId,
            int customerId, string commentText, DateTime createdOn)
        {
            return InsertBlogComment(blogPostId, customerId, commentText,
                createdOn, IoC.Resolve<IBlogService>().NotifyAboutNewBlogComments);
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
        public BlogComment InsertBlogComment(int blogPostId,
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
        public BlogComment InsertBlogComment(int blogPostId,
            int customerId, string ipAddress, string commentText, DateTime createdOn, bool notify)
        {
            ipAddress = CommonHelper.EnsureNotNull(ipAddress);
            ipAddress = CommonHelper.EnsureMaximumLength(ipAddress, 100);
            commentText = CommonHelper.EnsureNotNull(commentText);

            

            var blogComment = _context.BlogComments.CreateObject();
            blogComment.BlogPostId = blogPostId;
            blogComment.CustomerId = customerId;
            blogComment.IPAddress = ipAddress;
            blogComment.CommentText = commentText;
            blogComment.CreatedOn = createdOn;

            _context.BlogComments.AddObject(blogComment);
            _context.SaveChanges();

            if (notify)
            {
                IoC.Resolve<IMessageService>().SendBlogCommentNotificationMessage(blogComment, IoC.Resolve<ILocalizationManager>().DefaultAdminLanguage.LanguageId);
            }

            return blogComment;
        }

        /// <summary>
        /// Updates the blog comment
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        public void UpdateBlogComment(BlogComment blogComment)
        {
            if (blogComment == null)
                throw new ArgumentNullException("activityLogType");

            blogComment.IPAddress = CommonHelper.EnsureNotNull(blogComment.IPAddress);
            blogComment.IPAddress = CommonHelper.EnsureMaximumLength(blogComment.IPAddress, 100);
            blogComment.CommentText = CommonHelper.EnsureNotNull(blogComment.CommentText);

            
            if (!_context.IsAttached(blogComment))
                _context.BlogComments.Attach(blogComment);

            _context.SaveChanges();
        }
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.BlogManager.CacheEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether blog is enabled
        /// </summary>
        public bool BlogEnabled
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.EnableBlog");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Common.EnableBlog", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the page size for posts
        /// </summary>
        public int PostsPageSize
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueInteger("Blog.PostsPageSize", 10);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Blog.PostsPageSize", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether not registered user can leave comments
        /// </summary>
        public bool AllowNotRegisteredUsersToLeaveComments
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Blog.AllowNotRegisteredUsersToLeaveComments");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Blog.AllowNotRegisteredUsersToLeaveComments", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to notify about new blog comments
        /// </summary>
        public bool NotifyAboutNewBlogComments
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Blog.NotifyAboutNewBlogComments");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Blog.NotifyAboutNewBlogComments", value.ToString());
            }
        }
        #endregion
    }
}

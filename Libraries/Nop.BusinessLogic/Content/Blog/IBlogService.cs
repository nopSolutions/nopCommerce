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
using NopSolutions.NopCommerce.BusinessLogic.IoC;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.BusinessLogic.Content.Blog
{
    /// <summary>
    /// Blog post service interface
    /// </summary>
    public partial interface IBlogService
    {
        /// <summary>
        /// Deletes an blog post
        /// </summary>
        /// <param name="blogPostId">Blog post identifier</param>
        void DeleteBlogPost(int blogPostId);

        /// <summary>
        /// Gets an blog post
        /// </summary>
        /// <param name="blogPostId">Blog post identifier</param>
        /// <returns>Blog post</returns>
        BlogPost GetBlogPostById(int blogPostId);

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all records</param>
        /// <returns>Blog posts</returns>
        List<BlogPost> GetAllBlogPosts(int languageId);

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Blog posts</returns>
        PagedList<BlogPost> GetAllBlogPosts(int languageId, int pageIndex, int pageSize);

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="dateFrom">Filter by created date; null if you want to get all records</param>
        /// <param name="dateTo">Filter by created date; null if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Blog posts</returns>
        PagedList<BlogPost> GetAllBlogPosts(int languageId,
            DateTime? dateFrom, DateTime? dateTo, int pageIndex, int pageSize);

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="tag">Tag</param>
        /// <returns>Blog posts</returns>
        List<BlogPost> GetAllBlogPostsByTag(int languageId, string tag);

        /// <summary>
        /// Gets all blog post tags
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <returns>Blog post tags</returns>
        List<BlogPostTag> GetAllBlogPostTags(int languageId);

        /// <summary>
        /// Inserts an blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        void InsertBlogPost(BlogPost blogPost);

        /// <summary>
        /// Updates the blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        void UpdateBlogPost(BlogPost blogPost);

        /// <summary>
        /// Deletes an blog comment
        /// </summary>
        /// <param name="blogCommentId">Blog comment identifier</param>
        void DeleteBlogComment(int blogCommentId);

        /// <summary>
        /// Gets an blog comment
        /// </summary>
        /// <param name="blogCommentId">Blog comment identifier</param>
        /// <returns>A blog comment</returns>
        BlogComment GetBlogCommentById(int blogCommentId);
        /// <summary>
        /// Gets a collection of blog comments by blog post identifier
        /// </summary>
        /// <param name="blogPostId">Blog post identifier</param>
        /// <returns>A collection of blog comments</returns>
        List<BlogComment> GetBlogCommentsByBlogPostId(int blogPostId);

        /// <summary>
        /// Gets all blog comments
        /// </summary>
        /// <returns>Blog comments</returns>
        List<BlogComment> GetAllBlogComments();

        /// <summary>
        /// Inserts a blog comment
        /// </summary>
        /// <param name="blogPostId">The blog post identifier</param>
        /// <param name="customerId">The customer identifier who commented the blog post</param>
        /// <param name="commentText">The comment text</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <returns>Blog comment</returns>
        BlogComment InsertBlogComment(int blogPostId,
            int customerId, string commentText, DateTime createdOn);

        /// <summary>
        /// Inserts a blog comment
        /// </summary>
        /// <param name="blogPostId">The blog post identifier</param>
        /// <param name="customerId">The customer identifier who commented the blog post</param>
        /// <param name="commentText">The comment text</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="notify">A value indicating whether to notify the store owner</param>
        /// <returns>Blog comment</returns>
        BlogComment InsertBlogComment(int blogPostId,
            int customerId, string commentText, DateTime createdOn, bool notify);

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
        BlogComment InsertBlogComment(int blogPostId,
            int customerId, string ipAddress, string commentText, DateTime createdOn, bool notify);

        /// <summary>
        /// Updates the blog comment
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        void UpdateBlogComment(BlogComment blogComment);
        
        /// <summary>
        /// Gets or sets a value indicating whether blog is enabled
        /// </summary>
        bool BlogEnabled {get;set;}

        /// <summary>
        /// Gets or sets the page size for posts
        /// </summary>
        int PostsPageSize {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether not registered user can leave comments
        /// </summary>
        bool AllowNotRegisteredUsersToLeaveComments {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether to notify about new blog comments
        /// </summary>
        bool NotifyAboutNewBlogComments {get;set;}
    }
}

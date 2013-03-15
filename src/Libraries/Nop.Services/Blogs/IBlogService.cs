using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Blogs;

namespace Nop.Services.Blogs
{
    /// <summary>
    /// Blog service interface
    /// </summary>
    public partial interface IBlogService
    {
        /// <summary>
        /// Deletes a blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        void DeleteBlogPost(BlogPost blogPost);

        /// <summary>
        /// Gets a blog post
        /// </summary>
        /// <param name="blogPostId">Blog post identifier</param>
        /// <returns>Blog post</returns>
        BlogPost GetBlogPostById(int blogPostId);

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="dateFrom">Filter by created date; null if you want to get all records</param>
        /// <param name="dateTo">Filter by created date; null if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog posts</returns>
        IPagedList<BlogPost> GetAllBlogPosts(int storeId, int languageId,
            DateTime? dateFrom, DateTime? dateTo, int pageIndex, int pageSize, bool showHidden = false);

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="tag">Tag</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog posts</returns>
        IPagedList<BlogPost> GetAllBlogPostsByTag(int storeId, int languageId, string tag, 
            int pageIndex, int pageSize, bool showHidden = false);

        /// <summary>
        /// Gets all blog post tags
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog post tags</returns>
        IList<BlogPostTag> GetAllBlogPostTags(int storeId, int languageId, bool showHidden = false);

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
        /// Gets all comments
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <returns>Comments</returns>
        IList<BlogComment> GetAllComments(int customerId);

        /// <summary>
        /// Gets a blog comment
        /// </summary>
        /// <param name="blogCommentId">Blog comment identifier</param>
        /// <returns>Blog comment</returns>
        BlogComment GetBlogCommentById(int blogCommentId);

        /// <summary>
        /// Deletes a blog comment
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        void DeleteBlogComment(BlogComment blogComment);
    }
}

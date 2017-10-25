using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Blogs;

namespace Nop.Services.Blogs
{
    /// <summary>
    /// Blog service interface
    /// 博客服务接口
    /// </summary>
    public partial interface IBlogService
    {
        #region Blog posts, 博文操作

        /// <summary>
        /// Deletes a blog post
        /// 删除博文
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        void DeleteBlogPost(BlogPost blogPost);

        /// <summary>
        /// Gets a blog post
        /// 根据博文id获得一条博文实体
        /// </summary>
        /// <param name="blogPostId">Blog post identifier</param>
        /// <returns>Blog post</returns>
        BlogPost GetBlogPostById(int blogPostId);

        /// <summary>
        /// Gets blog posts
        /// 根据博文id集合，获得多条博文实体集合
        /// </summary>
        /// <param name="blogPostIds">Blog post identifiers</param>
        /// <returns>Blog posts</returns>
        IList<BlogPost> GetBlogPostsByIds(int[] blogPostIds);

        /// <summary>
        /// Gets all blog posts
        /// 获得所有博文，支持简单查询，支持分页
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="dateFrom">Filter by created date; null if you want to get all records</param>
        /// <param name="dateTo">Filter by created date; null if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog posts</returns>
        IPagedList<BlogPost> GetAllBlogPosts(int storeId = 0, int languageId = 0,
            DateTime? dateFrom = null, DateTime? dateTo = null, 
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets all blog posts
        /// 根据标签获得博文
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier. 0 if you want to get all blog posts</param>
        /// <param name="tag">Tag</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog posts</returns>
        IPagedList<BlogPost> GetAllBlogPostsByTag(int storeId = 0,
            int languageId = 0, string tag = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets all blog post tags
        /// 获得所有博文标签
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier. 0 if you want to get all blog posts</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog post tags</returns>
        IList<BlogPostTag> GetAllBlogPostTags(int storeId, int languageId, bool showHidden = false);

        /// <summary>
        /// Inserts an blog post
        /// 新增博文
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        void InsertBlogPost(BlogPost blogPost);

        /// <summary>
        /// Updates the blog post
        /// 更新博文
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        void UpdateBlogPost(BlogPost blogPost);

        #endregion

        #region Blog comments, 博文评论操作

        /// <summary>
        /// Gets all comments
        /// 获得所有评论
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="blogPostId">Blog post ID; 0 or null to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item creation to; null to load all records</param>
        /// <param name="commentText">Search comment text; null to load all records</param>
        /// <returns>Comments</returns>
        IList<BlogComment> GetAllComments(int customerId = 0, int storeId = 0, int? blogPostId = null,
            bool? approved = null, DateTime? fromUtc = null, DateTime? toUtc = null, string commentText = null);

        /// <summary>
        /// Gets a blog comment
        /// 获得一条评论
        /// </summary>
        /// <param name="blogCommentId">Blog comment identifier</param>
        /// <returns>Blog comment</returns>
        BlogComment GetBlogCommentById(int blogCommentId);

        /// <summary>
        /// Get blog comments by identifiers
        /// </summary>
        /// <param name="commentIds">Blog comment identifiers</param>
        /// <returns>Blog comments</returns>
        IList<BlogComment> GetBlogCommentsByIds(int[] commentIds);

        /// <summary>
        /// Get the count of blog comments
        /// 获得指定博文的评论的数量
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="isApproved">A value indicating whether to count only approved or not approved comments; pass null to get number of all comments</param>
        /// <returns>Number of blog comments</returns>
        int GetBlogCommentsCount(BlogPost blogPost, int storeId = 0, bool? isApproved = null);

        /// <summary>
        /// Deletes a blog comment
        /// 删除一条博文评论
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        void DeleteBlogComment(BlogComment blogComment);

        /// <summary>
        /// Deletes blog comments
        /// 批量删除博文评论
        /// </summary>
        /// <param name="blogComments">Blog comments</param>
        void DeleteBlogComments(IList<BlogComment> blogComments);

        #endregion
    }
}

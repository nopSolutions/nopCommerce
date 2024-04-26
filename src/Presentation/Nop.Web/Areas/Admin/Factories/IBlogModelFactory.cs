using Nop.Core.Domain.Blogs;
using Nop.Web.Areas.Admin.Models.Blogs;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the blog model factory
/// </summary>
public partial interface IBlogModelFactory
{
    /// <summary>
    /// Prepare blog content model
    /// </summary>
    /// <param name="blogContentModel">Blog content model</param>
    /// <param name="filterByBlogPostId">Blog post ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog content model
    /// </returns>
    Task<BlogContentModel> PrepareBlogContentModelAsync(BlogContentModel blogContentModel, int? filterByBlogPostId);

    /// <summary>
    /// Prepare paged blog post list model
    /// </summary>
    /// <param name="searchModel">Blog post search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog post list model
    /// </returns>
    Task<BlogPostListModel> PrepareBlogPostListModelAsync(BlogPostSearchModel searchModel);

    /// <summary>
    /// Prepare blog post model
    /// </summary>
    /// <param name="model">Blog post model</param>
    /// <param name="blogPost">Blog post</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog post model
    /// </returns>
    Task<BlogPostModel> PrepareBlogPostModelAsync(BlogPostModel model, BlogPost blogPost, bool excludeProperties = false);

    /// <summary>
    /// Prepare blog comment search model
    /// </summary>
    /// <param name="searchModel">Blog comment search model</param>
    /// <param name="blogPost">Blog post</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog comment search model
    /// </returns>
    Task<BlogCommentSearchModel> PrepareBlogCommentSearchModelAsync(BlogCommentSearchModel searchModel, BlogPost blogPost);

    /// <summary>
    /// Prepare paged blog comment list model
    /// </summary>
    /// <param name="searchModel">Blog comment search model</param>
    /// <param name="blogPostId">Blog post ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog comment list model
    /// </returns>
    Task<BlogCommentListModel> PrepareBlogCommentListModelAsync(BlogCommentSearchModel searchModel, int? blogPostId);

    /// <summary>
    /// Prepare blog post search model
    /// </summary>
    /// <param name="searchModel">Blog post search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog post search model
    /// </returns>
    Task<BlogPostSearchModel> PrepareBlogPostSearchModelAsync(BlogPostSearchModel searchModel);
}
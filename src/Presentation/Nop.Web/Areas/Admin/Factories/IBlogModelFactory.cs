using Nop.Core.Domain.Blogs;
using Nop.Web.Areas.Admin.Models.Blogs;

namespace Nop.Web.Areas.Admin.Factories
{
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
        /// <returns>Blog content model</returns>
        BlogContentModel PrepareBlogContentModel(BlogContentModel blogContentModel, int? filterByBlogPostId);

        /// <summary>
        /// Prepare blog post search model
        /// </summary>
        /// <param name="searchModel">Blog post search model</param>
        /// <returns>Blog post search model</returns>
        BlogPostSearchModel PrepareBlogPostSearchModel(BlogPostSearchModel searchModel);

        /// <summary>
        /// Prepare paged blog post list model
        /// </summary>
        /// <param name="searchModel">Blog post search model</param>
        /// <returns>Blog post list model</returns>
        BlogPostListModel PrepareBlogPostListModel(BlogPostSearchModel searchModel);

        /// <summary>
        /// Prepare blog post model
        /// </summary>
        /// <param name="model">Blog post model</param>
        /// <param name="blogPost">Blog post</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Blog post model</returns>
        BlogPostModel PrepareBlogPostModel(BlogPostModel model, BlogPost blogPost, bool excludeProperties = false);

        /// <summary>
        /// Prepare blog comment search model
        /// </summary>
        /// <param name="searchModel">Blog comment search model</param>
        /// <param name="blogPost">Blog post</param>
        /// <returns>Blog comment search model</returns>
        BlogCommentSearchModel PrepareBlogCommentSearchModel(BlogCommentSearchModel searchModel, BlogPost blogPost);

        /// <summary>
        /// Prepare paged blog comment list model
        /// </summary>
        /// <param name="searchModel">Blog comment search model</param>
        /// <param name="blogPostId">Blog post ID</param>
        /// <returns>Blog comment list model</returns>
        BlogCommentListModel PrepareBlogCommentListModel(BlogCommentSearchModel searchModel, int? blogPostId);
    }
}
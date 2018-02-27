using Nop.Core.Domain.Blogs;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Framework.Kendoui;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the blog model factory
    /// </summary>
    public partial interface IBlogModelFactory
    {
        /// <summary>
        /// Prepare blog post list model
        /// </summary>
        /// <param name="model">Blog post list model</param>
        /// <returns>Blog post list model</returns>
        BlogPostListModel PrepareBlogPostListModel(BlogPostListModel model);

        /// <summary>
        /// Prepare paged blog post list model for the grid
        /// </summary>
        /// <param name="listModel">Blog post list model</param>
        /// <param name="command">Pagination parameters</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareBlogPostListGridModel(BlogPostListModel listModel, DataSourceRequest command);

        /// <summary>
        /// Prepare blog post model
        /// </summary>
        /// <param name="model">Blog post model</param>
        /// <param name="blogPost">Blog post</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Blog post model</returns>
        BlogPostModel PrepareBlogPostModel(BlogPostModel model, BlogPost blogPost, bool excludeProperties = false);

        /// <summary>
        /// Prepare blog comment list model
        /// </summary>
        /// <param name="model">Blog comment list model</param>
        /// <param name="blogPost">Blog post</param>
        /// <returns>Blog comment list model</returns>
        BlogCommentListModel PrepareBlogCommentListModel(BlogCommentListModel model, BlogPost blogPost);

        /// <summary>
        /// Prepare paged blog comment list model for the grid
        /// </summary>
        /// <param name="listModel">Blog comment list model</param>
        /// <param name="command">Pagination parameters</param>
        /// <param name="blogPost">Blog post; pass null to load comments of all posts</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareBlogCommentListGridModel(BlogCommentListModel listModel,
            DataSourceRequest command, BlogPost blogPost);
    }
}
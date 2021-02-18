using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Blogs;
using Nop.Web.Models.Blogs;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the blog model factory
    /// </summary>
    public partial interface IBlogModelFactory
    {
        /// <summary>
        /// Prepare blog post model
        /// </summary>
        /// <param name="model">Blog post model</param>
        /// <param name="blogPost">Blog post entity</param>
        /// <param name="prepareComments">Whether to prepare blog comments</param>
        Task PrepareBlogPostModelAsync(BlogPostModel model, BlogPost blogPost, bool prepareComments);

        /// <summary>
        /// Prepare blog post list model
        /// </summary>
        /// <param name="command">Blog paging filtering model</param>
        /// <returns>Blog post list model</returns>
        Task<BlogPostListModel> PrepareBlogPostListModelAsync(BlogPagingFilteringModel command);

        /// <summary>
        /// Prepare blog post tag list model
        /// </summary>
        /// <returns>Blog post tag list model</returns>
        Task<BlogPostTagListModel> PrepareBlogPostTagListModelAsync();

        /// <summary>
        /// Prepare blog post year models
        /// </summary>
        /// <returns>List of blog post year model</returns>
        Task<List<BlogPostYearModel>> PrepareBlogPostYearModelAsync();
    }
}
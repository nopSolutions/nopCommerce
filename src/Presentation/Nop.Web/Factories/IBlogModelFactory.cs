using System.Collections.Generic;
using Nop.Core.Domain.Blogs;
using Nop.Web.Models.Blogs;

namespace Nop.Web.Factories
{
    public partial interface IBlogModelFactory
    {
        void PrepareBlogPostModel(BlogPostModel model, BlogPost blogPost, bool prepareComments);

        BlogPostListModel PrepareBlogPostListModel(BlogPagingFilteringModel command);

        BlogPostTagListModel PrepareBlogPostTagListModel();

        List<BlogPostYearModel> PrepareBlogPostYearModel();
    }
}

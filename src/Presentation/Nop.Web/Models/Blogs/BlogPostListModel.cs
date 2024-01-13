using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Blogs;

public partial record BlogPostListModel : BaseNopModel
{
    public BlogPostListModel()
    {
        PagingFilteringContext = new BlogPagingFilteringModel();
        BlogPosts = new List<BlogPostModel>();
    }

    public int WorkingLanguageId { get; set; }
    public BlogPagingFilteringModel PagingFilteringContext { get; set; }
    public IList<BlogPostModel> BlogPosts { get; set; }
}
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Blogs
{
    /// <summary>
    /// Represents a blog content model
    /// </summary>
    public partial record BlogContentModel : BaseNopModel
    {
        #region Ctor

        public BlogContentModel()
        {
            BlogPosts = new BlogPostSearchModel();
            BlogComments = new BlogCommentSearchModel();
            SearchTitle = new BlogPostSearchModel().SearchTitle;
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.List.SearchTitle")]
        public string SearchTitle { get; set; }

        public BlogPostSearchModel BlogPosts { get; set; }

        public BlogCommentSearchModel BlogComments { get; set; }

        #endregion
    }
}

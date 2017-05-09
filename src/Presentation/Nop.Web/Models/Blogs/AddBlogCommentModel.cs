#if NET451using System.Web.Mvc;
#endif
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.Blogs
{
    public partial class AddBlogCommentModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Blog.Comments.CommentText")]
        public string CommentText { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}
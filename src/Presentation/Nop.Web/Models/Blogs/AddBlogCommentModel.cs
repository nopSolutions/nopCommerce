using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Blogs
{
    public partial class AddBlogCommentModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Blog.Comments.CommentText")]
        [AllowHtml]
        public string CommentText { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}
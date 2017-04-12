#if NET451using System.Web.Mvc;
#endif
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Blogs
{
    public partial class AddBlogCommentModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Blog.Comments.CommentText")]
        	
#if NET451		[AllowHtml]
#endif
        public string CommentText { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}
#if NET451using System.Web.Mvc;
#endif
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.News
{
    public partial class AddNewsCommentModel : BaseNopModel
    {
        [NopResourceDisplayName("News.Comments.CommentTitle")]
        	
#if NET451
		[AllowHtml]
#endif
        public string CommentTitle { get; set; }

        [NopResourceDisplayName("News.Comments.CommentText")]
        	
#if NET451
		[AllowHtml]
#endif
        public string CommentText { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.News
{
    public partial class AddNewsCommentModel : BaseNopModel
    {
        [NopResourceDisplayName("News.Comments.CommentTitle")]
        public string CommentTitle { get; set; }

        [NopResourceDisplayName("News.Comments.CommentText")]
        public string CommentText { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.News;

public partial record AddNewsCommentModel : BaseNopModel
{
    [NopResourceDisplayName("News.Comments.CommentTitle")]
    public string CommentTitle { get; set; }

    [NopResourceDisplayName("News.Comments.CommentText")]
    public string CommentText { get; set; }

    public bool DisplayCaptcha { get; set; }
}
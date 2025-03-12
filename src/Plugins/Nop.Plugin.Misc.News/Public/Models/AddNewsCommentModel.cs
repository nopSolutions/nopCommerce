using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.News.Public.Models;

public partial record AddNewsCommentModel : BaseNopModel
{
    [NopResourceDisplayName("Plugins.Misc.News.Comments.CommentTitle")]
    public string CommentTitle { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Comments.CommentText")]
    public string CommentText { get; set; }

    public bool DisplayCaptcha { get; set; }
}
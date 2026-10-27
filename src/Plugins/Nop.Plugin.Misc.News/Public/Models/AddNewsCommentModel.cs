using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.News.Public.Models;

/// <summary>
/// Represents the add news comment model
/// </summary>
public record AddNewsCommentModel : BaseNopModel
{
    #region Properties

    [NopResourceDisplayName("Plugins.Misc.News.Comments.CommentTitle")]
    public string CommentTitle { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Comments.CommentText")]
    public string CommentText { get; set; }

    public bool DisplayCaptcha { get; set; }

    #endregion
}
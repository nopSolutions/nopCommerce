using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.News.Public.Models;

/// <summary>
/// Represents the news comment model
/// </summary>
public record NewsCommentModel : BaseNopEntityModel
{
    #region Properties

    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerAvatarUrl { get; set; }
    public string CommentTitle { get; set; }
    public string CommentText { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool AllowViewingProfiles { get; set; }

    #endregion
}
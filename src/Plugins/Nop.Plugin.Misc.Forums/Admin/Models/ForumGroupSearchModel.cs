using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Forums.Admin.Models;

/// <summary>
/// Represents a forum group search model
/// </summary>
public record ForumGroupSearchModel : BaseSearchModel
{
    #region Ctor

    public ForumGroupSearchModel()
    {
        ForumSearch = new ForumSearchModel();
    }

    #endregion

    #region Properties

    public ForumSearchModel ForumSearch { get; set; }

    #endregion
}
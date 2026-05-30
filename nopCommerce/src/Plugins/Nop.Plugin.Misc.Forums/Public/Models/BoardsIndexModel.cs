using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Forums.Public.Models;

public record BoardsIndexModel : BaseNopModel
{
    #region Properties

    public List<ForumGroupModel> ForumGroups { get; set; } = new();

    #endregion
}
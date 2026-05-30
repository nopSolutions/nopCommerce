using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Forums.Public.Models;

public record ForumGroupModel : BaseNopEntityModel
{
    #region Properties

    public string Name { get; set; }
    public string SeName { get; set; }

    public List<ForumRowModel> Forums { get; set; } = new();

    #endregion
}
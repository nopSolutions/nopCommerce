using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Forums.Admin.Models;

/// <summary>
/// Represents a forum group model
/// </summary>
public record ForumGroupModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Plugins.Misc.Forums.ForumGroup.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.ForumGroup.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.ForumGroup.Fields.CreatedOn")]
    public DateTime CreatedOn { get; set; }

    #endregion
}
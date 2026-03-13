using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Forums.Admin.Models;

/// <summary>
/// Represents a forum list model
/// </summary>
public record ForumModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Plugins.Misc.Forums.Forum.Fields.ForumGroupId")]
    public int ForumGroupId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Forum.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Forum.Fields.Description")]
    public string Description { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Forum.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Forum.Fields.CreatedOn")]
    public DateTime CreatedOn { get; set; }

    public List<SelectListItem> ForumGroups { get; set; } = new();

    #endregion
}
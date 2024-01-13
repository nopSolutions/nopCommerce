using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an URL record model
/// </summary>
public partial record UrlRecordModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Admin.System.SeNames.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.System.SeNames.EntityId")]
    public int EntityId { get; set; }

    [NopResourceDisplayName("Admin.System.SeNames.EntityName")]
    public string EntityName { get; set; }

    [NopResourceDisplayName("Admin.System.SeNames.IsActive")]
    public bool IsActive { get; set; }

    [NopResourceDisplayName("Admin.System.SeNames.Language")]
    public string Language { get; set; }

    [NopResourceDisplayName("Admin.System.SeNames.Details")]
    public string DetailsUrl { get; set; }

    #endregion
}
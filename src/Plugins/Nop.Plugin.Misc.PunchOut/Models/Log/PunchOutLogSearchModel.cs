using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.PunchOut.Models.Log;

/// <summary>
/// Represents PunchOutLog search model
/// </summary>
public record PunchOutLogSearchModel : BaseSearchModel
{
    [NopResourceDisplayName("Plugins.Misc.PunchOut.Log.Search.CreatedFrom")]
    [UIHint("DateNullable")]
    public DateTime? CreatedFrom { get; set; }

    [NopResourceDisplayName("Plugins.Misc.PunchOut.Log.Search.CreatedTo")]
    [UIHint("DateNullable")]
    public DateTime? CreatedTo { get; set; }
    public bool HideSearchBlock { get; set; }
}

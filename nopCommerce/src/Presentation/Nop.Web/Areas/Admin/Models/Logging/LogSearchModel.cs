using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Logging;

/// <summary>
/// Represents a log search model
/// </summary>
public partial record LogSearchModel : BaseSearchModel
{
    #region Ctor

    public LogSearchModel()
    {
        AvailableLogLevels = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.System.Log.List.CreatedOnFrom")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnFrom { get; set; }

    [NopResourceDisplayName("Admin.System.Log.List.CreatedOnTo")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnTo { get; set; }

    [NopResourceDisplayName("Admin.System.Log.List.Message")]
    public string Message { get; set; }

    [NopResourceDisplayName("Admin.System.Log.List.LogLevel")]
    public int LogLevelId { get; set; }

    public IList<SelectListItem> AvailableLogLevels { get; set; }

    #endregion
}
using Newtonsoft.Json;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents a license check model
/// </summary>
public partial record LicenseCheckModel : BaseNopModel
{
    #region Properties

    [JsonProperty(PropertyName = "display_warning")]
    public bool? DisplayWarning { get; set; }

    [JsonProperty(PropertyName = "block_pages")]
    public bool? BlockPages { get; set; }

    [JsonProperty(PropertyName = "warning_text")]
    public string WarningText { get; set; }

    #endregion
}
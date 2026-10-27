using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.PunchOut.Models.Log;

/// <summary>
/// Represents a punch out log model
/// </summary>
public record PunchOutLogModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Plugins.Misc.PunchOut.Log.SessionId")]
    public string SessionId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.PunchOut.Log.MessageType")]
    public string MessageType { get; set; }

    [NopResourceDisplayName("Plugins.Misc.PunchOut.Log.Direction")]
    public string Direction { get; set; }

    [NopResourceDisplayName("Plugins.Misc.PunchOut.Log.RawXml")]
    public string RawXml { get; set; }

    [NopResourceDisplayName("Plugins.Misc.PunchOut.Log.Url")]
    public string Url { get; set; }

    [NopResourceDisplayName("Plugins.Misc.PunchOut.Log.Identity")]
    public string Identity { get; set; }

    [NopResourceDisplayName("Plugins.Misc.PunchOut.Log.Error")]
    public string Error { get; set; }

    [NopResourceDisplayName("Plugins.Misc.PunchOut.Log.CreatedDate")]
    public DateTime CreatedDate { get; set; }

    #endregion
}

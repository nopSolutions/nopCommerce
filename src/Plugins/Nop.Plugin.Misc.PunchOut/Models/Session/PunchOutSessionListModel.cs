using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.PunchOut.Models.Session;

/// <summary>
/// Represents a punch out session list model
/// </summary>
public record PunchOutSessionListModel : BasePagedListModel<PunchOutSessionModel>;

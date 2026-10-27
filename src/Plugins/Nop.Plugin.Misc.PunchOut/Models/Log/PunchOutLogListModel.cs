
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.PunchOut.Models.Log;

/// <summary>
/// Represents a punch out log list model
/// </summary>
public record PunchOutLogListModel : BasePagedListModel<PunchOutLogModel>;

using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.PunchOut.Models.Identity;

/// <summary>
/// Represents a punch out identity list model
/// </summary>
public record PunchOutIdentityListModel : BasePagedListModel<PunchOutIdentityModel>;
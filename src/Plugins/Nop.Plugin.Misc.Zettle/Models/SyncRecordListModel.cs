using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Zettle.Models;

/// <summary>
/// Represents a synchronization record list model
/// </summary>
public record SyncRecordListModel : BasePagedListModel<SyncRecordModel>
{
}
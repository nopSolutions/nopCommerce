using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a filter level list model
/// </summary>
public partial record FilterLevelListModel : BasePagedListModel<FilterLevelModel>
{
}
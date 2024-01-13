using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Logging;

/// <summary>
/// Represents a log list model
/// </summary>
public partial record LogListModel : BasePagedListModel<LogModel>
{
}
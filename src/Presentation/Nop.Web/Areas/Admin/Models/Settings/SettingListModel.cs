using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a setting list model
/// </summary>
public partial record SettingListModel : BasePagedListModel<SettingModel>
{
}
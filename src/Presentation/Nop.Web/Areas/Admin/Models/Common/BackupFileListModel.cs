using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents a backup file list model
/// </summary>
public partial record BackupFileListModel : BasePagedListModel<BackupFileModel>
{
}
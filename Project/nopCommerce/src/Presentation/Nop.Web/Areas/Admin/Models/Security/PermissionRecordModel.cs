using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Security;

/// <summary>
/// Represents a permission record model
/// </summary>
public partial record PermissionRecordModel : BaseNopModel
{
    #region Properties

    public string Name { get; set; }

    public string SystemName { get; set; }

    #endregion
}
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Security;

/// <summary>
/// Represents a permission configuration model
/// </summary>
public partial record PermissionConfigurationModel : BaseNopModel
{
    #region Ctor

    public PermissionConfigurationModel()
    {
        PermissionCategorySearchModel = new PermissionCategorySearchModel
        {
            Length = int.MaxValue
        };
    }

    #endregion

    #region Properties

    public bool IsPermissionsAvailable { get; set; }

    public bool AreCustomerRolesAvailable { get; set; }

    public PermissionCategorySearchModel PermissionCategorySearchModel { get; set; }

    #endregion
}
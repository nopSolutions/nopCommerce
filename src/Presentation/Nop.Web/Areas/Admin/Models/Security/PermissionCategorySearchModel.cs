using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Security;

public partial record PermissionCategorySearchModel : BaseSearchModel
{
    #region Ctor

    public PermissionCategorySearchModel()
    {
        PermissionItemSearchModel = new PermissionItemSearchModel();
    }

    #endregion

    #region Properties

    public PermissionItemSearchModel PermissionItemSearchModel { get; set; }

    #endregion
}
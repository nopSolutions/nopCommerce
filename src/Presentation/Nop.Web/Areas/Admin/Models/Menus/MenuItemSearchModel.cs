using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Menus;

/// <summary>
/// Represents menu items search model
/// </summary>
public partial record MenuItemSearchModel : BaseSearchModel
{
    #region Properties

    public int MenuId { get; set; }

    #endregion
}

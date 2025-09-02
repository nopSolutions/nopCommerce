using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Menus;

/// <summary>
/// Represents a select entity model
/// </summary>
public partial record SelectMenuItemEntityModel : BaseNopModel
{
    #region Properties

    public int MenuItemId { get; set; }

    public int EntityId { get; set; }

    #endregion
}

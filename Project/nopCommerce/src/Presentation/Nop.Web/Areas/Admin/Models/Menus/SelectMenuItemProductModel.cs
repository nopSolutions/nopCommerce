using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Menus;

public partial record SelectMenuItemProductModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItems.SelectProduct.Fields.ProductName")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItems.SelectProduct.Fields.Published")]
    public bool Published { get; set; }

    #endregion
}

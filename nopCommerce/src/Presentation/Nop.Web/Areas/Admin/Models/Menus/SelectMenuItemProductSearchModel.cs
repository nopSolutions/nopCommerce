using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Menus;

public partial record SelectMenuItemProductSearchModel : BaseSearchModel
{
    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItems.SelectProduct.SearchKeywords")]
    public string SearchKeywords { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItems.SelectProduct.SearchStore")]
    public int SearchStoreId { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; } = new List<SelectListItem>();

    public bool HideStoresList { get; set; }

    public SelectMenuItemEntityModel SelectMenuItemModel { get; set; } = new();

    #endregion
}

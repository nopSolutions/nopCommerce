using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Menus;

/// <summary>
/// Represents menu item model
/// </summary>
public partial record MenuItemModel : BaseNopEntityModel, IAclSupportedModel, IStoreMappingSupportedModel, ILocalizedModel<MenuItemLocalizedModel>
{
    #region Ctor

    public MenuItemModel()
    {
        AvailableMenuItemTypes = new List<SelectListItem>();
        AvailableMenuItems = new List<SelectListItem>();

        SelectedStoreIds = new List<int>();
        AvailableStores = new List<SelectListItem>();

        SelectedCustomerRoleIds = new List<int>();
        AvailableCustomerRoles = new List<SelectListItem>();

        AvailableMenuItemTemplates = new List<SelectListItem>();
        AvailableStandardRoutes = new List<SelectListItem>();
        AvailableCategories = new List<SelectListItem>();
        AvailableVendors = new List<SelectListItem>();
        AvailableManufacturers = new List<SelectListItem>();
        AvailableTopics = new List<SelectListItem>();

        Locales = new List<MenuItemLocalizedModel>();
    }

    #endregion

    #region Properties

    public int MenuId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.Title")]
    public string Title { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.Breadcrumb")]
    public string Breadcrumb { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.Url")]
    public string Url { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.Category")]
    public int CategoryId { get; set; }
    public IList<SelectListItem> AvailableCategories { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.Vendor")]
    public int VendorId { get; set; }
    public IList<SelectListItem> AvailableVendors { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.Manufacturer")]
    public int ManufacturerId { get; set; }
    public IList<SelectListItem> AvailableManufacturers { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.Topic")]
    public int TopicId { get; set; }
    public IList<SelectListItem> AvailableTopics { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.Product")]
    public int? ProductId { get; set; }
    public string ProductName { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.MenuItemType")]
    public int MenuItemTypeId { get; set; }
    public string MenuItemTypeName { get; set; }
    public IList<SelectListItem> AvailableMenuItemTypes { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.RouteName")]
    public string RouteName { get; set; }
    public IList<SelectListItem> AvailableStandardRoutes { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.Template")]
    public int TemplateId { get; set; }
    public IList<SelectListItem> AvailableMenuItemTemplates { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.Published")]
    public bool Published { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.CssClass")]
    public string CssClass { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.Parent")]
    public int? ParentId { get; set; }
    public IList<SelectListItem> AvailableMenuItems { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.NumberOfSubItemsPerGridElement")]
    public int? NumberOfSubItemsPerGridElement { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.NumberOfItemsPerGridRow")]
    public int? NumberOfItemsPerGridRow { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.MaximumNumberEntities")]
    public int? MaximumNumberEntities { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }
    public IList<SelectListItem> AvailableStores { get; set; }

    public IList<int> SelectedCustomerRoleIds { get; set; }
    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    public IList<MenuItemLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record MenuItemLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.MenuItem.Fields.Title")]
    public string Title { get; set; }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.PriceLists;

/// <summary>
/// Represents a price list search model
/// </summary>
public partial record PriceListSearchModel : BaseSearchModel, IAclSupportedModel
{
    #region Ctor

    public PriceListSearchModel()
    {
        SelectedCustomerRoleIds = new List<int>();
        AvailableCustomerRoles = new List<SelectListItem>();
        AvailableActiveValues = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Catalog.PriceLists.List.CustomerRoles")]
    public IList<int> SelectedCustomerRoleIds { get; set; }

    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    [NopResourceDisplayName("Admin.Catalog.PriceLists.List.SearchIsActive")]
    public bool? SearchIsActive { get; set; } = true;
    public IList<SelectListItem> AvailableActiveValues { get; set; }

    public bool HidePriority { get; set; }

    #endregion
}

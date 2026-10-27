using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.PriceLists;

/// <summary>
/// Represents a price list model
/// </summary>
public partial record PriceListModel : BaseNopEntityModel, IAclSupportedModel
{
    #region Ctor

    public PriceListModel()
    {
        SelectedCustomerRoleIds = new List<int>();
        AvailableCustomerRoles = new List<SelectListItem>();
        PriceListItemSearchModel = new PriceListItemSearchModel();
        PriceListCustomerSearchModel = new PriceListCustomerSearchModel();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Catalog.PriceLists.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.PriceLists.Fields.Description")]
    public string Description { get; set; }

    [NopResourceDisplayName("Admin.Catalog.PriceLists.Fields.Active")]
    public bool Active { get; set; }

    [NopResourceDisplayName("Admin.Catalog.PriceLists.Fields.StartDateUtc")]
    [UIHint("DateTimeNullable")]
    public DateTime? StartDateUtc { get; set; }

    [NopResourceDisplayName("Admin.Catalog.PriceLists.Fields.EndDateUtc")]
    [UIHint("DateTimeNullable")]
    public DateTime? EndDateUtc { get; set; }

    [NopResourceDisplayName("Admin.Catalog.PriceLists.Fields.PriceCalculationTypeId")]
    public int PriceCalculationTypeId { get; set; }
    public string PriceCalculationTypeName { get; set; }

    [NopResourceDisplayName("Admin.Catalog.PriceLists.Fields.PriceCalculationValue")]
    public decimal PriceCalculationValue { get; set; }
    public string PriceCalculationValueFormatted { get; set; }

    [NopResourceDisplayName("Admin.Catalog.PriceLists.Fields.Priority")]
    public int Priority { get; set; }

    public bool HidePriority { get; set; }

    //ACL (customer roles)
    [NopResourceDisplayName("Admin.Catalog.PriceLists.Fields.CustomerRoles")]
    public IList<int> SelectedCustomerRoleIds { get; set; }

    [NopResourceDisplayName("Admin.Catalog.PriceLists.Fields.CustomerRoles")]
    public string CustomerRoleNames { get; set; }

    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    public PriceListItemSearchModel PriceListItemSearchModel { get; set; }
    public PriceListCustomerSearchModel PriceListCustomerSearchModel { get; set; }

    #endregion
}

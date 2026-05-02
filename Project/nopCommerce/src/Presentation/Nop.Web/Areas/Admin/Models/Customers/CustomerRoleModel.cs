using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a customer role model
/// </summary>
public partial record CustomerRoleModel : BaseNopEntityModel
{
    #region Ctor

    public CustomerRoleModel()
    {
        TaxDisplayTypeValues = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Customers.CustomerRoles.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerRoles.Fields.FreeShipping")]
    public bool FreeShipping { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerRoles.Fields.TaxExempt")]
    public bool TaxExempt { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerRoles.Fields.Active")]
    public bool Active { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerRoles.Fields.IsSystemRole")]
    public bool IsSystemRole { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerRoles.Fields.SystemName")]
    public string SystemName { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerRoles.Fields.EnablePasswordLifetime")]
    public bool EnablePasswordLifetime { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerRoles.Fields.OverrideTaxDisplayType")]
    public bool OverrideTaxDisplayType { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerRoles.Fields.DefaultTaxDisplayType")]
    public int DefaultTaxDisplayTypeId { get; set; }

    public IList<SelectListItem> TaxDisplayTypeValues { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct")]
    public int PurchasedWithProductId { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct")]
    public string PurchasedWithProductName { get; set; }

    #endregion
}
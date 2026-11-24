using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Affiliates;

/// <summary>
/// Represents an affiliate model
/// </summary>
public partial record AffiliateModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Admin.Affiliates.Fields.URL")]
    public string Url { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Fields.CreatedOn")]
    public DateTime CreatedOn { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Fields.AdminComment")]
    public string AdminComment { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Fields.FriendlyUrlName")]
    public string FriendlyUrlName { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Fields.Active")]
    public bool Active { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Fields.AssociatedCustomerId")]
    public int? AssociatedCustomerId { get; set; }
    public string CustomerInfo { get; set; }

    public bool OrdersExists { get; set; }

    public AddressModel Address { get; set; } = new();

    public AffiliatedOrderSearchModel AffiliatedOrderSearchModel { get; set; } = new();

    public AffiliateCommissionSearchModel AffiliateCommissionSearchModel { get; set; } = new();

    public AffiliatedCustomerSearchModel AffiliatedCustomerSearchModel { get; set; } = new();

    #endregion
}


/// <summary>
/// Represents an affiliate commission model to add new one
/// </summary>
public partial record AffiliateCommissionEditModel : AffiliateCommissionModel
{
    #region Properties

    public int AffiliateId { get; set; }

    #endregion
}
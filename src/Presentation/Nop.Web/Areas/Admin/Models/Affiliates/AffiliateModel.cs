using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Affiliates;

/// <summary>
/// Represents an affiliate model
/// </summary>
public partial record AffiliateModel : BaseNopEntityModel
{
    #region Ctor

    public AffiliateModel()
    {
        Address = new AddressModel();
        AffiliatedOrderSearchModel = new AffiliatedOrderSearchModel();
        AffiliateCommissionSearchModel = new AffiliateCommissionSearchModel();
        AffiliatedCustomerSearchModel = new AffiliatedCustomerSearchModel();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Affiliates.Fields.URL")]
    public string Url { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Fields.AdminComment")]
    public string AdminComment { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Fields.FriendlyUrlName")]
    public string FriendlyUrlName { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Fields.Active")]
    public bool Active { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Fields.AssociatedCustomerId")]
    public int? AssociatedCustomerId { get; set; }
    public string CustomerInfo { get; set; }

    public AddressModel Address { get; set; }

    public AffiliatedOrderSearchModel AffiliatedOrderSearchModel { get; set; }

    public AffiliateCommissionSearchModel AffiliateCommissionSearchModel { get; set; }

    public AffiliatedCustomerSearchModel AffiliatedCustomerSearchModel { get; set; }

    #endregion
}


/// <summary>
/// Represents an affiliate commission model to add new one
/// </summary>
public partial record AffiliateCommissionEditModel : AffiliateCommissionModel
{
    #region Ctor

    public AffiliateCommissionEditModel()
    {
        AffiliatedOrderSearchModel = new AffiliatedOrderSearchModel();
    }

    #endregion

    #region Properties

    public int AffiliateId { get; set; }

    public AffiliatedOrderSearchModel AffiliatedOrderSearchModel { get; set; }

    #endregion
}
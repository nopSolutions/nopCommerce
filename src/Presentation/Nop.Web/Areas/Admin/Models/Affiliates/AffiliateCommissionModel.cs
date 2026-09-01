using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Affiliates;

/// <summary>
/// Represents an affiliated commission model
/// </summary>
public partial record AffiliateCommissionModel : BaseNopEntityModel
{
    #region Properties

    public override int Id { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Commission.TotalCommissionAmount")]
    public string TotalCommissionAmount { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Commission.CommissionStatus")]
    public string CommissionStatus { get; set; }
    public string CommissionStatusId { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Commission.PaidOn")]
    public DateTime? PaidOn { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Commission.CreateOn")]
    public DateTime CreateOn { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Commission.AdminComment")]
    public string AdminComment { get; set; }

    #endregion
}
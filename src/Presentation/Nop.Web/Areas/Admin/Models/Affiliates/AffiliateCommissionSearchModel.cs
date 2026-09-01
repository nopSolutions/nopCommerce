using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Affiliates;

/// <summary>
/// Represents an affiliate commission search model
/// </summary>
public partial record AffiliateCommissionSearchModel : BaseSearchModel
{
    #region Ctor

    public AffiliateCommissionSearchModel()
    {
        AvailableCommissionStatus = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    public int AffliateId { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Commissions.StartDate")]
    [UIHint("DateNullable")]
    public DateTime? StartDate { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Commissions.EndDate")]
    [UIHint("DateNullable")]
    public DateTime? EndDate { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Commissions.CommissionStatus")]
    public int CommissionStatusId { get; set; }

    public IList<SelectListItem> AvailableCommissionStatus { get; set; }

    #endregion
}
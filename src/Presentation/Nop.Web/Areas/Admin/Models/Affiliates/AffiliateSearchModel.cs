using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Affiliates;

/// <summary>
/// Represents an affiliate search model
/// </summary>
public partial record AffiliateSearchModel : BaseSearchModel
{
    #region Properties

    [NopResourceDisplayName("Admin.Affiliates.List.SearchFirstName")]
    public string SearchFirstName { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.List.SearchLastName")]
    public string SearchLastName { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.List.SearchFriendlyUrlName")]
    public string SearchFriendlyUrlName { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.List.LoadOnlyWithOrders")]
    public bool LoadOnlyWithOrders { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.List.OrdersCreatedFromUtc")]
    [UIHint("DateNullable")]
    public DateTime? OrdersCreatedFromUtc { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.List.OrdersCreatedToUtc")]
    [UIHint("DateNullable")]
    public DateTime? OrdersCreatedToUtc { get; set; }

    #endregion
}
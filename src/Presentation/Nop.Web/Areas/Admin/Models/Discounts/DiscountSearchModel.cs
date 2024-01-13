using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Discounts;

/// <summary>
/// Represents a discount search model
/// </summary>
public partial record DiscountSearchModel : BaseSearchModel
{
    #region Ctor

    public DiscountSearchModel()
    {
        AvailableActiveOptions = new List<SelectListItem>();
        AvailableDiscountTypes = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Promotions.Discounts.List.SearchDiscountCouponCode")]
    public string SearchDiscountCouponCode { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Discounts.List.SearchDiscountName")]
    public string SearchDiscountName { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Discounts.List.SearchDiscountType")]
    public int SearchDiscountTypeId { get; set; }

    public IList<SelectListItem> AvailableDiscountTypes { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Discounts.List.SearchStartDate")]
    [UIHint("DateNullable")]
    public DateTime? SearchStartDate { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Discounts.List.SearchEndDate")]
    [UIHint("DateNullable")]
    public DateTime? SearchEndDate { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Discounts.List.IsActive")]
    public int IsActiveId { get; set; }

    public IList<SelectListItem> AvailableActiveOptions { get; set; }

    #endregion
}
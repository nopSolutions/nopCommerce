using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Affiliate;

/// <summary>
/// Represent the affiliate model
/// </summary>
public record AffiliateModel : BaseNopModel
{
    public AffiliateModel()
    {
        Orders = new List<AffiliatedOrderModel>();
    }

    [NopResourceDisplayName("Account.Affiliates.Status")]
    public string Status { get; set; }

    [NopResourceDisplayName("Account.Affiliates.TotalAffiliatedCustomers")]
    public int TotalAffiliatedCustomers { get; set; }
    public IList<SelectListItem> AvailableLimits { get; set; }
    [NopResourceDisplayName("Account.Affiliates.TotalOrders")]
    public int TotalOrders { get; set; }

    [NopResourceDisplayName("Account.Affiliates.OrdersAmount")]
    public string OrdersAmount { get; set; }
    [NopResourceDisplayName("Account.Affiliates.OrdersCount")]
    public int OrdersCount { get; set; }

    [NopResourceDisplayName("Account.Affiliates.TotalCommission")]
    public string TotalCommission { get; set; }
    [NopResourceDisplayName("Account.Affiliates.PaidCommission")]
    public string PaidCommission { get; set; }

    public PagerModel PagerModel { get; set; }
    public List<AffiliatedOrderModel> Orders { get; set; }
}
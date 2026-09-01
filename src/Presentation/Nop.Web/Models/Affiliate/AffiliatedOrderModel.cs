using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Affiliate;

/// <summary>
/// Represents the affiliate order model
/// </summary>
public partial record AffiliatedOrderModel : BaseNopModel
{
    public string CustomOrderNumber { get; set; }
    public string OrderTotal { get; set; }
    public string OrderStatus { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime CreatedOn { get; set; }
    public string AffiliateCommission { get; set; }
    public string CommissionStatus { get; set; }
    public DateTime? CommissionPaidOn { get; set; }
}
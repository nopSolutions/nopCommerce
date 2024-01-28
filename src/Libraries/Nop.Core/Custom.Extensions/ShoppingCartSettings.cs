using Nop.Core.Configuration;

namespace Nop.Core.Domain.Orders;

/// <summary>
/// Shopping cart settings
/// </summary>
public partial class ShoppingCartSettings
{
    public int OneMonthSubscriptionProductId { get; set; }
    public int ThreeMonthSubscriptionProductId { get; set; }
    public int SixMonthSubscriptionProductId { get; set; }
    public int OneYearSubscriptionProductId { get; set; }

    public int OneMonthSubscriptionAllottedCount { get; set; }
    public int ThreeMonthSubscriptionAllottedCount { get; set; }
    public int SixMonthSubscriptionAllottedCount { get; set; }
    public int OneYearSubscriptionAllottedCount { get; set; }

}
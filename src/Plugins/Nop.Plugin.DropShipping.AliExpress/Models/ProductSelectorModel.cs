namespace Nop.Plugin.DropShipping.AliExpress.Models;

/// <summary>
/// Model for the product selector widget
/// </summary>
public class ProductSelectorModel
{
    public int ProductId { get; set; }
    public long? AliExpressProductId { get; set; }
    public string? AliExpressProductTitle { get; set; }
    public string? AliExpressProductUrl { get; set; }
    public string? AliExpressImageUrl { get; set; }
    public decimal? AliExpressPrice { get; set; }
    public bool HasMapping { get; set; }
}

/// <summary>
/// Model for product price calculation
/// </summary>
public class PriceCalculationModel
{
    public long AliExpressProductId { get; set; }
    public decimal ProductPrice { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal VatPercentage { get; set; }
    public decimal CustomsDutyPercentage { get; set; }
    public decimal MarginPercentage { get; set; }
    
    public decimal CustomsDuty => (ProductPrice + ShippingCost) * (CustomsDutyPercentage / 100);
    public decimal Subtotal => ProductPrice + ShippingCost + CustomsDuty;
    public decimal VatAmount => Subtotal * (VatPercentage / 100);
    public decimal TotalBeforeMargin => Subtotal + VatAmount;
    public decimal FinalPrice => TotalBeforeMargin * (1 + MarginPercentage / 100);
}

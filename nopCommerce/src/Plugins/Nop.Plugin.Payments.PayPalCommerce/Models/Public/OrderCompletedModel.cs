namespace Nop.Plugin.Payments.PayPalCommerce.Models.Public;

/// <summary>
/// Represents the order completed model
/// </summary>
public record OrderCompletedModel : OrderModel
{
    public string Warning { get; set; }
}
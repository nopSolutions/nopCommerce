using Nop.Plugin.Payments.PayPalCommerce.Domain;

namespace Nop.Plugin.Payments.PayPalCommerce.Models.Public;

/// <summary>
/// Represents the Apple Pay model
/// </summary>
public record ApplePayModel : OrderModel
{
    #region Properties

    public ButtonPlacement Placement { get; set; }

    public string CurrencyCode { get; set; }

    public Contact BillingAddress { get; set; } = new();

    public Contact ShippingAddress { get; set; } = new();

    public List<(string Id, string Label, string Description, string Price)> ShippingOptions { get; set; } = new();

    public List<(string Type, string Price, string Status, string Label)> Items { get; set; } = new();

    #endregion
}
namespace Nop.Plugin.Payments.PayPalCommerce.Models.Public;

/// <summary>
/// Represents the order shipping model
/// </summary>
public record OrderShippingModel : OrderModel
{
    #region Properties

    public string PaymentId { get; set; }

    public string PaymentToken { get; set; }

    public string AddressCity { get; set; }

    public string AddressState { get; set; }

    public string AddressCountryCode { get; set; }

    public string AddressPostalCode { get; set; }

    public string OptionId { get; set; }

    public string OptionType { get; set; }

    public string OptionLabel { get; set; }

    public bool OptionSelected { get; set; }

    public decimal OptionAmount { get; set; }

    #endregion
}
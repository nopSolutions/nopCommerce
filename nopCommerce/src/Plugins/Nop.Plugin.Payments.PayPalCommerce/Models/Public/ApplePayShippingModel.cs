namespace Nop.Plugin.Payments.PayPalCommerce.Models.Public;

/// <summary>
/// Represents the Apple Pay shipping model
/// </summary>
public record ApplePayShippingModel : ApplePayModel
{
    #region Properties

    public string AddressCity { get; set; }

    public string AddressState { get; set; }

    public string AddressCountryCode { get; set; }

    public string AddressPostalCode { get; set; }

    public string OptionId { get; set; }

    #endregion
}
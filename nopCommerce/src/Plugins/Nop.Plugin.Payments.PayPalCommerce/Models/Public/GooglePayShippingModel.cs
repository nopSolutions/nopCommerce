namespace Nop.Plugin.Payments.PayPalCommerce.Models.Public;

/// <summary>
/// Represents the Google Pay shipping model
/// </summary>
public record GooglePayShippingModel : GooglePayModel
{
    #region Properties

    public string AddressCity { get; set; }

    public string AddressState { get; set; }

    public string AddressCountryCode { get; set; }

    public string AddressPostalCode { get; set; }

    public string OptionId { get; set; }

    public List<(string Id, string Label, string Description)> Options { get; set; } = new();

    #endregion
}
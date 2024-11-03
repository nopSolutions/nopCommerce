namespace Nop.Plugin.Payments.PayPalCommerce.Models.Public;

/// <summary>
/// Represents the order confirmation model
/// </summary>
public record OrderConfirmModel : OrderModel
{
    #region Properties

    public string OrderGuid { get; set; }

    public string LiabilityShift { get; set; }

    public bool TermsOfServiceOnOrderConfirmPage { get; set; }

    public bool TermsOfServicePopup { get; set; }

    public string MinOrderTotalWarning { get; set; }

    public bool DisplayCaptcha { get; set; }

    public IList<string> Warnings { get; set; } = new List<string>();

    #endregion
}
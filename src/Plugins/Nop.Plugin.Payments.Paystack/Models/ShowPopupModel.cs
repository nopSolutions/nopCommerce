namespace Nop.Plugin.Payments.Paystack.Models;

/// <summary>
/// Model for showing Paystack popup page
/// </summary>
public class ShowPopupModel
{
    /// <summary>
    /// Paystack access code
    /// </summary>
    public string AccessCode { get; set; } = string.Empty;

    /// <summary>
    /// Transaction reference
    /// </summary>
    public string Reference { get; set; } = string.Empty;

    /// <summary>
    /// Order ID
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Callback URL
    /// </summary>
    public string CallbackUrl { get; set; } = string.Empty;
}

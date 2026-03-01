using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Paystack.Models;

/// <summary>
/// Represents the Paystack callback/redirect query parameters
/// </summary>
public record PaystackCallbackModel : BaseNopModel
{
    /// <summary>
    /// Gets or sets the transaction reference from Paystack
    /// </summary>
    public string Reference { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the trxref (transaction reference) when different from reference
    /// </summary>
    public string Trxref { get; set; } = string.Empty;
}

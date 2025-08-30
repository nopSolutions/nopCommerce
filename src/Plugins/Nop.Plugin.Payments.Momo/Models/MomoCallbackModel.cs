using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Momo.Models;

/// <summary>
/// Represents a MoMo callback request
/// </summary>
public record MomoCallbackModel : BaseNopModel
{
    /// <summary>
    /// Gets or sets the reference ID
    /// </summary>
    public string ReferenceId { get; set; }

    /// <summary>
    /// Gets or sets the status
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the reason (for failures)
    /// </summary>
    public string Reason { get; set; }

    /// <summary>
    /// Gets or sets the transaction ID from MTN
    /// </summary>
    public string MomoTransactionId { get; set; }

    /// <summary>
    /// Gets or sets the payment date
    /// </summary>
    public string PaymentDate { get; set; }

    /// <summary>
    /// Gets or sets the external reference ID
    /// </summary>
    public string ExternalId { get; set; }

    /// <summary>
    /// Gets or sets the signature for callback validation
    /// </summary>
    public string Signature { get; set; }
}

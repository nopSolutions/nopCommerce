using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Paystack.Models;

/// <summary>
/// Represents the payment info model for Paystack checkout
/// </summary>
public record PaymentInfoModel : BaseNopModel
{
    [NopResourceDisplayName("Plugins.Payments.Paystack.Fields.Email")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether email is required (set by processor)
    /// </summary>
    public bool EmailRequired { get; internal set; }

    /// <summary>
    /// Gets or sets the payment note shown to the customer (set by processor)
    /// </summary>
    public string PaymentNote { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer email when already known (set by processor)
    /// </summary>
    public string CustomerEmail { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets or sets the order total display (set by processor)
    /// </summary>
    public string OrderTotal { get; internal set; } = string.Empty;
}

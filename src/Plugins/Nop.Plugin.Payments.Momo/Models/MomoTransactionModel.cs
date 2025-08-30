using System;
using Nop.Core;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Momo.Models;

/// <summary>
/// Represents a MoMo transaction
/// </summary>
public class MomoTransactionModel : BaseEntity
{
    /// <summary>
    /// Gets or sets the reference ID (from MTN MoMo)
    /// </summary>
    public string ReferenceId { get; set; }

    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the transaction amount
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the currency code
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Gets or sets the creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the transaction status
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the error message (if any)
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the associated order ID
    /// </summary>
    public int OrderId { get; set; }
}

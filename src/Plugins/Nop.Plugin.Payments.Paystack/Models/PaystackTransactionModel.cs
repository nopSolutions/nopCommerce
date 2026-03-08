using System;
using Nop.Core;

namespace Nop.Plugin.Payments.Paystack.Models;

/// <summary>
/// Represents a Paystack transaction (persisted for reference/order linking)
/// </summary>
public class PaystackTransactionModel : BaseEntity
{
    /// <summary>
    /// Gets or sets the transaction reference from Paystack
    /// </summary>
    public string Reference { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer email
    /// </summary>
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the transaction amount
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the currency code
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the transaction status (e.g. success, failed, abandoned)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error or gateway message (if any)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the associated order ID
    /// </summary>
    public int OrderId { get; set; }
}

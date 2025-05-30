using Nop.Core;

namespace Nop.Plugin.Payments.PayPalCommerce.Domain;

/// <summary>
/// Represents the PayPal payment token
/// </summary>
public class PayPalToken : BaseEntity
{
    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the Vault identifier
    /// </summary>
    public string VaultId { get; set; }

    /// <summary>
    /// Gets or sets the unique ID for a customer in PayPal Vault
    /// </summary>
    public string VaultCustomerId { get; set; }

    /// <summary>
    /// Gets or sets the related transaction identifier
    /// </summary>
    public string TransactionId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the token is a primary customer's payment method
    /// </summary>
    public bool IsPrimaryMethod { get; set; }

    /// <summary>
    /// Gets or sets the token type
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the merchant client id
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the payment method title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the payment method expiration
    /// </summary>
    public string Expiration { get; set; }
}
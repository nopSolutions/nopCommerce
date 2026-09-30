namespace Nop.Plugin.Api.Rest.Models.Requests;

/// <summary>
/// Request to cancel an order
/// </summary>
public record CancelOrderRequest
{
    /// <summary>
    /// Gets or sets a value indicating whether the customer is notified by email
    /// </summary>
    public bool NotifyCustomer { get; set; }
}

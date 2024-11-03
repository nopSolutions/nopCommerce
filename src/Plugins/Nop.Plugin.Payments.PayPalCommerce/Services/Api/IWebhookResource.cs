namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api;

/// <summary>
/// Represents a webhook resource
/// </summary>
public interface IWebhookResource
{
    /// <summary>
    /// Gets or sets the API caller-provided external ID
    /// </summary>
    string CustomId { get; set; }
}
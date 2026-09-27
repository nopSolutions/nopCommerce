using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.WorldpayHpp.Models;
public class WorldpayHppSettings : ISettings
{
    public string EntityId { get; set; }           // Worldpay eCommerce service key
    public string Username { get; set; }            // If needed for browser-side features
    public string Password { get; set; }         // For HPP setup
    public string Environment { get; set; }          // "TEST" or "LIVE"
    public string NarrativeLine1 { get; set; } // Identification of the merchant on the cardholder's bank statement.
    public string TransactionReferencePrefix { get; set; } = string.Empty;
    //public bool UseIframe { get; set; }              // Use iframe/lightbox vs redirect
    public string SuccessUrl { get; set; }           // Override if needed
    public string CancelUrl { get; set; }            // Override if needed
    public string FailureUrl { get; set; }           // Override if needed
    public string ErrorUrl { get; set; }            // Override if needed
    public string ExpiryUrl { get; set; }            // Override if needed
    public string PendingUrl { get; set; }            // Override if needed
}

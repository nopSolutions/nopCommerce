namespace Nop.Plugin.Payments.WorldpayHpp.Models;
public class ConfigurationModel
{
    public string NarrativeLine1 { get; set; }
    public string EntityId { get; set; }
    public string TransactionReferencePrefix { get; set; } = string.Empty;
    public string Username { get; set; }
    public string Password { get; set; }
    public string Environment { get; set; }
    //public bool UseIframe { get; set; }
    public string SuccessUrl { get; set; }
    public string PendingUrl { get; set; }
    public string FailureUrl { get; set; }
    public string ErrorUrl { get; set; }
    public string CancelUrl { get; set; }
    public string ExpiryUrl { get; set; }
    
}

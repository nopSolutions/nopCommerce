namespace Nop.Plugin.Payments.WorldpayHpp.Models.Requests;
public class PaymentRequest
{
    public string TransactionReference { get; set; } 
    public Merchant Merchant { get; set; } 
    public Narrative Narrative { get; set; } 
    public Value Value { get; set; } 
    public string Description { get; set; } 
    //public string BillingAddressName { get; set; } 
   public BillingAddress BillingAddress { get; set; } 
    public ResultUrls ResultURLs { get; set; }
    //public RiskData RiskData { get; set; } 
    //public HostedCustomization HostedCustomization { get; set; } 
   // public HostedProperties HostedProperties { get; set; } = new HostedProperties();
    //public Settlement Settlement { get; set; } 
    //public string Expiry { get; set; }
#if DEBUG
    public ThreeDS ThreeDS { get; set; } = new ThreeDS();
    public Fraud Fraud { get; set; } = new Fraud();
#endif
}

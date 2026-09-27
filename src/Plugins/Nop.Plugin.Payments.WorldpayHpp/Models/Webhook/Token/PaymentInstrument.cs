namespace Nop.Plugin.Payments.WorldpayHpp.Models.Webhook.Token;
public class PaymentInstrument
{
    public string CardNumber { get; set; }
    public string CardHolderName { get; set; }
    public string ExpiryMonth { get; set; }
    public string ExpiryYear { get; set; }
    public string CardType { get; set; }
}

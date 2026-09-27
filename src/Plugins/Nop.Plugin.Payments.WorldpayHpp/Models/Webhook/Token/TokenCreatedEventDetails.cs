namespace Nop.Plugin.Payments.WorldpayHpp.Models.Webhook.Token;
public class TokenCreatedEventDetails
{
    public string TransactionReference { get; set; }
    public DateTime TokenCreatedAt { get; set; }
    public TokenPaymentInstrument TokenPaymentInstrument { get; set; }
    public PaymentInstrument PaymentInstrument { get; set; }
    public BillingAddress BillingAddress { get; set; }
    public string Namespace { get; set; }
    public string SchemeTransactionReference { get; set; }
}

namespace Nop.Plugin.Payments.WorldpayHpp.Models.Webhook.Token;
public class TokenPaymentInstrument
{
    public string Type { get; set; }
    public string Href { get; set; }
    public string TokenId { get; set; }
    public string Description { get; set; }
    public DateTime TokenExpiryDateTime { get; set; }
}

namespace Nop.Plugin.Payments.WorldpayHpp.Models.Webhook.Payment;
public class RefundInfo
{
    public string OnlineRefundAuthorization { get; set; }
    public RefundRefusalInfo Refusal { get; set; }
}

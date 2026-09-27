namespace Nop.Plugin.Payments.WorldpayHpp.Models.Webhook.Payment;
public class PaymentEventDetails
{
    public string Classification { get; set; }
    public string DownstreamReference { get; set; }
    public string TransactionReference { get; set; }
    public string Type { get; set; }
    public DateTime Date { get; set; }
    public string Reference { get; set; } // nullable
    public string OctReference { get; set; } // nullable
    public AmountInfo Amount { get; set; } // nullable
    public RefundInfo Refund { get; set; } // nullable
    public PaymentLinks _links { get; set; }
}

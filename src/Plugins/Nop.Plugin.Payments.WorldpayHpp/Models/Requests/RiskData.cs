namespace Nop.Plugin.Payments.WorldpayHpp.Models.Requests;
public class RiskData
{
    public Shipping Shipping { get; set; }
    public Custom Custom { get; set; }
    public Account Account { get; set; }
    public Transaction Transaction { get; set; }
}
namespace Nop.Plugin.Payments.WorldpayHpp.Models.Requests;
public class TransactionHistory
{
    public string AttemptsLastYear { get; set; }
    public string CompletedLastSixMonths { get; set; }
    public string AttemptsLastDay { get; set; }
    public string ShippingAddressFirstUsedAt { get; set; }
    public string AddCardsLastDay { get; set; }
}
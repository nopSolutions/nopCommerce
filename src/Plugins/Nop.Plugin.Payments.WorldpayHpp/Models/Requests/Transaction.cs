namespace Nop.Plugin.Payments.WorldpayHpp.Models.Requests;
public class Transaction
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string PreOrderDate { get; set; }
    public string Reorder { get; set; }
    public TransactionHistory History { get; set; }
    public GiftCardsPurchase GiftCardsPurchase { get; set; }
}

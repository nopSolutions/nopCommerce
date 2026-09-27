namespace Nop.Plugin.Payments.WorldpayHpp.Models.Requests;
public class BillingAddress
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string Address3 { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string CountryCode { get; set; }
}

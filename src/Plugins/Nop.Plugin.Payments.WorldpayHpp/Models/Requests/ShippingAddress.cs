namespace Nop.Plugin.Payments.WorldpayHpp.Models.Requests;
public class ShippingAddress
{
    public string City { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string Address3 { get; set; }
    public string State { get; set; }
    public string CountryCode { get; set; }
    public string PostalCode { get; set; }
    public string PhoneNumber { get; set; }
}

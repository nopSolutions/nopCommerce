namespace Nop.Plugin.Payments.WorldpayHpp.Models.Requests;
public class Shipping
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ShippingAddress Address { get; set; }
    public string Method { get; set; }
    public string NameMatchesAccountName { get; set; }
    public string Email { get; set; }
    public string TimeFrame { get; set; }
}

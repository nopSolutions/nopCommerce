namespace Nop.Plugin.Payments.WorldpayHpp.Models.Requests;
public class Account
{
    public string ShopperId { get; set; }
    public string DateOfBirth { get; set; }
    public AccountHistory History { get; set; }
    public string Type { get; set; }
    public string PreviousSuspiciousActivity { get; set; }
    public string Email { get; set; }
}

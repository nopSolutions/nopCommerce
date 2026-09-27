namespace Nop.Plugin.Payments.WorldpayHpp.Models.Requests;
public class ResultUrls
{
    public string SuccessURL { get; set; }
    public string PendingURL { get; set; }
    public string FailureURL { get; set; }
    public string ErrorURL { get; set; }
    public string CancelURL { get; set; }
    public string ExpiryURL { get; set; }
}
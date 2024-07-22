using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.AmazonPay.Models;

public record SignInModel : BaseNopModel
{
    public string AmazonPayScript { get; set; }

    public string ButtonColor { get; set; }

    public string LedgerCurrency { get; set; }

    public string Payload { get; set; }

    public string Signature { get; set; }
}
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.AmazonPay.Models;

public record ChangeLinkModel : BaseNopModel
{
    public string CheckoutSessionId { get; set; }
    public string ChangeAction { get; set; }
}

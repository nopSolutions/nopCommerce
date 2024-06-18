using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Bitcoin.Models;

public record BitcoinPaymentInfoModel : BaseNopModel
{
    public string SomeText { get; set; }
}
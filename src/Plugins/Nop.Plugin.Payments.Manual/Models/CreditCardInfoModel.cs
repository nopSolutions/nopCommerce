using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Manual.Models;

public record CreditCardInfoModel: BaseNopModel
{
    public int OrderId { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Manual.CardType")]
    public string CardType { get; set; }
    [NopResourceDisplayName("Plugins.Payments.Manual.CardName")]
    public string CardName { get; set; }
    [NopResourceDisplayName("Plugins.Payments.Manual.CardNumber")]
    public string CardNumber { get; set; }
    [NopResourceDisplayName("Plugins.Payments.Manual.CardCVV2")]
    public string CardCvv2 { get; set; }
    [NopResourceDisplayName("Plugins.Payments.Manual.CardExpirationMonth")]
    public string CardExpirationMonth { get; set; }
    [NopResourceDisplayName("Plugins.Payments.Manual.CardExpirationYear")]
    public string CardExpirationYear { get; set; }
}

using Nop.Plugin.Payments.AmazonPay.Enums;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.AmazonPay.Models;

public record PaymentInfoModel : BaseNopModel
{
    public string AmazonPayScript { get; set; }

    public string LedgerCurrency { get; set; }

    public string Currency { get; set; }

    public string ProductType { get; set; }

    public string ButtonColor { get; set; }

    public int? ProductId { get; set; }

    public ButtonPlacement Placement { get; set; }
    public string AmazonPayPlacement => Placement switch
    {
        ButtonPlacement.Cart or ButtonPlacement.Checkout or ButtonPlacement.Product => Placement.ToString(),
        ButtonPlacement.PaymentMethod => ButtonPlacement.Checkout.ToString(),
        ButtonPlacement.MiniCart => ButtonPlacement.Cart.ToString(),
        _ => "Other",
    };

    public bool IsCartContainsNoAllowedProducts { get; set; }
}
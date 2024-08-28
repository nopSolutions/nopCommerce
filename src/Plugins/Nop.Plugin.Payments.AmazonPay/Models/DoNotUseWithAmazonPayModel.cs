using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.AmazonPay.Models;

public record DoNotUseWithAmazonPayModel : BaseNopEntityModel
{
    [NopResourceDisplayName("Plugins.Payments.AmazonPay.DoNotUseWithAmazonPay")]
    public bool DoNotUseWithAmazonPay { get; set; }
}

using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.AmazonPay.Models;

public record CheckoutCompletedModel : BaseNopModel
{
    public int OrderId { get; set; }
    public string CustomOrderNumber { get; set; }
    public string AmazonPayScript { get; set; }
    public string BuyerId { get; set; }
}
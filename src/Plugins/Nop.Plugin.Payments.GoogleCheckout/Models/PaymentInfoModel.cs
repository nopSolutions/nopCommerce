using GCheckout;
using GCheckout.Checkout;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.GoogleCheckout.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        public string GifFileName { get; set; }

        public BackgroundType BackgroundType { get; set; }

        public string MerchantId { get; set; }

        public string MerchantKey { get; set; }

        public EnvironmentType Environment { get; set; }

        public string Currency { get; set; }

        public int CartExpirationMinutes { get; set; }

        public bool UseHttps { get; set; }

    }
}
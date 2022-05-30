using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Checkout
{
    public partial record OnePageCheckoutModel : BaseNopModel
    {
        public bool ShippingRequired { get; set; }
        public bool DisableBillingAddressCheckoutStep { get; set; }
        public bool DisplayCaptcha { get; set; }
        public bool IsReCaptchaV3 { get; set; }
        public string ReCaptchaPublicKey { get; set; }

        public CheckoutBillingAddressModel BillingAddress { get; set; }
    }
}
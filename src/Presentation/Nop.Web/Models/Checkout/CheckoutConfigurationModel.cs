namespace Nop.Web.Models.Checkout;

public partial record CheckoutCofigurationModel
{
    public bool DisableBillingAddressSection { get; init; }

    public bool DisplayCaptcha { get; init; }

    public bool IsReCaptchaV3 { get; init; }

    public string ReCaptchaPublicKey { get; init; }
}

using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Checkout;

public partial record CheckoutConfirmModel : BaseNopModel
{
    public CheckoutConfirmModel()
    {
        Warnings = new List<string>();
    }

    public bool TermsOfServiceOnOrderConfirmPage { get; set; }
    public bool TermsOfServicePopup { get; set; }
    public string MinOrderTotalWarning { get; set; }
    public bool DisplayCaptcha { get; set; }

    public IList<string> Warnings { get; set; }
}
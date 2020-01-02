using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Checkout
{
    public partial class CheckoutConfirmModel : BaseNopModel
    {
        public CheckoutConfirmModel()
        {
            Warnings = new List<string>();
        }

        public bool TermsOfServiceOnOrderConfirmPage { get; set; }
        public bool TermsOfServicePopup { get; set; }
        public string MinOrderTotalWarning { get; set; }

        public IList<string> Warnings { get; set; }
    }
}
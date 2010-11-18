using System;
using NopSolutions.NopCommerce.BusinessLogic.Payment;

namespace NopSolutions.NopCommerce.Web.Templates.Payment.Svea
{
    public partial class HostedPayment: BaseNopFrontendUserControl, IPaymentMethodModule
    {
        public PaymentInfo GetPaymentInfo()
        {
            return new PaymentInfo();
        }

        public bool ValidateForm()
        {
            return true;
        }
    }
}
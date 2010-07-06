using NopSolutions.NopCommerce.BusinessLogic.Payment;

namespace NopSolutions.NopCommerce.Web.Templates.Payment.Dibs
{
    public partial class FlexWinPayment : BaseNopUserControl, IPaymentMethodModule
    {
        #region Methods
        public PaymentInfo GetPaymentInfo()
        {
            return new PaymentInfo();
        }

        public bool ValidateForm()
        {
            return true;
        }
        #endregion
    }
}
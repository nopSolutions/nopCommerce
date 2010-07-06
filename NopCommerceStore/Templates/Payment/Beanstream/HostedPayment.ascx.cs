using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Payment;

namespace NopSolutions.NopCommerce.Web.Templates.Payment.Beanstream
{
    public partial class HostedPayment : BaseNopUserControl, IPaymentMethodModule
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
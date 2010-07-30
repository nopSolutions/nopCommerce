using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Payment;

namespace NopSolutions.NopCommerce.Web.Templates.Payment.SecurePay
{
    public partial class XmlPayment : BaseNopUserControl, IPaymentMethodModule
    {
        protected override void OnInit(EventArgs e)
        {
            BindData();
            base.OnInit(e);
        }

        private void BindData()
        {
            for(int i = 0; i < 15; i++)
            {
                string year = Convert.ToString(DateTime.Now.Year + i);
                creditCardExpireYear.Items.Add(new ListItem(year, year));
            }

            for(int i = 1; i <= 12; i++)
            {
                string text = (i < 10) ? "0" + i.ToString() : i.ToString();
                creditCardExpireMonth.Items.Add(new ListItem(text, i.ToString()));
            }
        }

        public bool ValidateForm()
        {
            return (this.CCNameValidator.IsValid &&
                this.CCValidator.IsValid &&
                this.CCRequiredValidator.IsValid &&
                this.rfvCVV2.IsValid &&
                this.revCVV2.IsValid);
        }

        public PaymentInfo GetPaymentInfo()
        {
            PaymentInfo paymentInfo = new PaymentInfo();
            paymentInfo.CreditCardType = this.ddlCreditCardType.SelectedItem.Value;
            paymentInfo.CreditCardName = this.creditCardName.Text;
            paymentInfo.CreditCardNumber = this.creditCardNumber.Text;
            paymentInfo.CreditCardExpireYear = int.Parse((this.creditCardExpireYear.SelectedValue == null) ? "0" : this.creditCardExpireYear.SelectedValue);
            paymentInfo.CreditCardExpireMonth = int.Parse((this.creditCardExpireMonth.SelectedValue == null) ? "0" : this.creditCardExpireMonth.SelectedValue);
            paymentInfo.CreditCardCvv2 = this.creditCardCVV2.Text;
            return paymentInfo;
        }

    }
}
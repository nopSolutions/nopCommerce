//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Payment;




namespace NopSolutions.NopCommerce.Web.Templates.Payment.PayJunction
{
    public partial class PaymentModule : BaseNopUserControl, IPaymentMethodModule
    {
        protected override void OnInit(EventArgs e)
        {
            BindData();
            base.OnInit(e);
        }

        private void BindData()
        {
            for (int i = 0; i < 15; i++)
            {
                string year = Convert.ToString(DateTime.Now.Year + i);
                creditCardExpireYear.Items.Add(new ListItem(year, year));
            }

            for (int i = 1; i <= 12; i++)
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
            paymentInfo.CreditCardName = this.creditCardName.Text;
            paymentInfo.CreditCardNumber = this.creditCardNumber.Text;
            paymentInfo.CreditCardExpireYear = int.Parse((this.creditCardExpireYear.SelectedValue == null) ? "0" : this.creditCardExpireYear.SelectedValue);
            paymentInfo.CreditCardExpireMonth = int.Parse((this.creditCardExpireMonth.SelectedValue == null) ? "0" : this.creditCardExpireMonth.SelectedValue);
            paymentInfo.CreditCardCvv2 = this.creditCardCVV2.Text;
            return paymentInfo;
        }

    }
}

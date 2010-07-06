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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.PayPal;
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.PayPalExpress
{
    public partial class ConfigurePaymentMethod : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        public static TransactMode GetCurrentTransactionMode()
        {
            TransactMode transactionModeEnum = TransactMode.Authorize;
            string transactionMode = SettingManager.GetSettingValue("PaymentMethod.PaypalExpress.TransactionMode");
            if (!String.IsNullOrEmpty(transactionMode))
            {
                transactionModeEnum = (TransactMode)Enum.Parse(typeof(TransactMode), transactionMode);
            }

            return transactionModeEnum;
        }

        public static void SetTransactionMode(TransactMode transactionMode)
        {
            SettingManager.SetParam("PaymentMethod.PaypalExpress.TransactionMode", transactionMode.ToString());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                BindData();
        }

        private void BindData()
        {
            TransactMode transactionModeEnum = GetCurrentTransactionMode();
            switch (transactionModeEnum)
            {
                case TransactMode.Authorize:
                    rbAuthorize.Checked = true;
                    break;
                case TransactMode.AuthorizeAndCapture:
                    rbAuthorizeAndCapture.Checked = true;
                    break;
                default:
                    break;
            }

            cbUseSandbox.Checked = SettingManager.GetSettingValueBoolean("PaymentMethod.PaypalExpress.UseSandbox");
            txtAPIAccountName.Text = SettingManager.GetSettingValue("PaymentMethod.PaypalExpress.APIAccountName");
            txtAPIAccountPassword.Text = SettingManager.GetSettingValue("PaymentMethod.PaypalExpress.APIAccountPassword");
            txtSignature.Text = SettingManager.GetSettingValue("PaymentMethod.PaypalExpress.Signature");
        }

        public void Save()
        {
            TransactMode transactionMode = TransactMode.Authorize;
            if (rbAuthorize.Checked)
            {
                transactionMode = TransactMode.Authorize;
            }
            if (rbAuthorizeAndCapture.Checked)
            {
                transactionMode = TransactMode.AuthorizeAndCapture;
            }
            SetTransactionMode(transactionMode);

            SettingManager.SetParam("PaymentMethod.PaypalExpress.UseSandbox", cbUseSandbox.Checked.ToString());
            SettingManager.SetParam("PaymentMethod.PaypalExpress.APIAccountName", txtAPIAccountName.Text);
            SettingManager.SetParam("PaymentMethod.PaypalExpress.APIAccountPassword", txtAPIAccountPassword.Text);
            SettingManager.SetParam("PaymentMethod.PaypalExpress.Signature", txtSignature.Text);
        }
    }
}

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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Templates.Payment;
using NopSolutions.NopCommerce.Payment.Methods.PayFlowPro;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.PayFlowPro
{
    public partial class ConfigurePaymentMethod : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        public static TransactMode GetCurrentTransactionMode()
        {
            TransactMode transactionModeEnum = TransactMode.Authorize;
            string transactionMode = SettingManager.GetSettingValue("PaymentMethod.PayFlowPro.TransactionMode");
            if (!String.IsNullOrEmpty(transactionMode))
            {
                transactionModeEnum = (TransactMode)Enum.Parse(typeof(TransactMode), transactionMode);
            }

            return transactionModeEnum;
        }

        public static void SetTransactionMode(TransactMode transactionMode)
        {
            SettingManager.SetParam("PaymentMethod.PayFlowPro.TransactionMode", transactionMode.ToString());
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

            cbUseSandbox.Checked = SettingManager.GetSettingValueBoolean("PaymentMethod.PayFlowPro.UseSandbox");
            txtUser.Text = SettingManager.GetSettingValue("PaymentMethod.PayFlowPro.User");
            txtVendor.Text = SettingManager.GetSettingValue("PaymentMethod.PayFlowPro.Vendor");
            txtPartner.Text = SettingManager.GetSettingValue("PaymentMethod.PayFlowPro.Partner");
            txtPassword.Text = SettingManager.GetSettingValue("PaymentMethod.PayFlowPro.Password");
            txtAdditionalFee.Value = SettingManager.GetSettingValueDecimalNative("PaymentMethod.PayFlowPro.AdditionalFee");
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

            SettingManager.SetParam("PaymentMethod.PayFlowPro.UseSandbox", cbUseSandbox.Checked.ToString());
            SettingManager.SetParam("PaymentMethod.PayFlowPro.User", txtUser.Text);
            SettingManager.SetParam("PaymentMethod.PayFlowPro.Vendor", txtVendor.Text);
            SettingManager.SetParam("PaymentMethod.PayFlowPro.Partner", txtPartner.Text);
            SettingManager.SetParam("PaymentMethod.PayFlowPro.Password", txtPassword.Text);
            SettingManager.SetParamNative("PaymentMethod.PayFlowPro.AdditionalFee", txtAdditionalFee.Value);
        }
    }
}

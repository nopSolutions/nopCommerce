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
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.Manual;
using NopSolutions.NopCommerce.Payment.Methods.QuickPay;
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.QuickPay
{
    public partial class ConfigurePaymentMethod : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            cbUseSandbox.Checked = SettingManager.GetSettingValueBoolean(QuickPayConstants.SETTING_USE_SANDBOX, true);
            txtMD5Secret.Text = SettingManager.GetSettingValue(QuickPayConstants.SETTING_MD5SECRET);
            txtCreditCard.Text = SettingManager.GetSettingValue(QuickPayConstants.SETTING_CREDITCARD_CODE_PROPERTY, "dankort");
            txtMerchantId.Text = SettingManager.GetSettingValue(QuickPayConstants.SETTING_MERCHANTID);
            txtAdditionalFee.Value = SettingManager.GetSettingValueDecimalNative("PaymentMethod.QuickPay.AdditionalFee", decimal.Zero);
        }

        public void Save()
        {
            SettingManager.SetParam(QuickPayConstants.SETTING_USE_SANDBOX, cbUseSandbox.Checked.ToString());
            SettingManager.SetParam(QuickPayConstants.SETTING_MD5SECRET, txtMD5Secret.Text);
            SettingManager.SetParam(QuickPayConstants.SETTING_MERCHANTID, txtMerchantId.Text);
            SettingManager.SetParam(QuickPayConstants.SETTING_CREDITCARD_CODE_PROPERTY, txtCreditCard.Text);
            SettingManager.SetParamNative("PaymentMethod.QuickPay.AdditionalFee", txtAdditionalFee.Value);
        }
    }
}

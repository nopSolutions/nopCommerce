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
using NopSolutions.NopCommerce.Payment.Methods.Worldpay;
using NopSolutions.NopCommerce.Web.Templates.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.Worldpay
{
    public partial class ConfigurePaymentMethod : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                BindData();
        }

        private void BindData()
        {
            txtWorldPayCSSName.Text = this.SettingManager.GetSettingValue(WorldpayConstants.SETTING_WorldPayCSSName);
            cbUseSandbox.Checked = this.SettingManager.GetSettingValueBoolean(WorldpayConstants.SETTING_USE_SANDBOX);
            txtWorldpayInstanceId.Text = this.SettingManager.GetSettingValue(WorldpayConstants.SETTING_INSTANCEID);
            txtCallbackPassword.Text = this.SettingManager.GetSettingValue(WorldpayConstants.SETTING_CALLBACK_PASSWORD);
            txtCreditCard.Text = this.SettingManager.GetSettingValue(WorldpayConstants.SETTING_CREDITCARD_CODE_PROPERTY);
            txtAdditionalFee.Value = this.SettingManager.GetSettingValueDecimalNative("PaymentMethod.Worldpay.AdditionalFee");
        }

        public void Save()
        {
            this.SettingManager.SetParam(WorldpayConstants.SETTING_WorldPayCSSName, txtWorldPayCSSName.Text);
            this.SettingManager.SetParam(WorldpayConstants.SETTING_USE_SANDBOX, cbUseSandbox.Checked.ToString());
            this.SettingManager.SetParam(WorldpayConstants.SETTING_INSTANCEID, txtWorldpayInstanceId.Text);
            this.SettingManager.SetParam(WorldpayConstants.SETTING_CALLBACK_PASSWORD, txtCallbackPassword.Text);
            this.SettingManager.SetParam(WorldpayConstants.SETTING_CREDITCARD_CODE_PROPERTY, txtCreditCard.Text);
            this.SettingManager.SetParamNative("PaymentMethod.Worldpay.AdditionalFee", txtAdditionalFee.Value);
        }
    }
}

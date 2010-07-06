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
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.PSIGate
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
            cbUseSandbox.Checked = SettingManager.GetSettingValueBoolean("PaymentMethod.PSIGate.UseSandbox");
            txtStoreId.Text = SettingManager.GetSettingValue("PaymentMethod.PSIGate.StoreId");
            txtPassphrase.Text = SettingManager.GetSettingValue("PaymentMethod.PSIGate.Passphrase");
            txtAdditionalFee.Value = SettingManager.GetSettingValueDecimalNative("PaymentMethod.PSIGate.AdditionalFee");
        }

        public void Save()
        {
            SettingManager.SetParam("PaymentMethod.PSIGate.UseSandbox", cbUseSandbox.Checked.ToString());
            SettingManager.SetParam("PaymentMethod.PSIGate.StoreId", txtStoreId.Text);
            SettingManager.SetParam("PaymentMethod.PSIGate.Passphrase", txtPassphrase.Text);
            SettingManager.SetParamNative("PaymentMethod.PSIGate.AdditionalFee", txtAdditionalFee.Value);
        }
    }
}

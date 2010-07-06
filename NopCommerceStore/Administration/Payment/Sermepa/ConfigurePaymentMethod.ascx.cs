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
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.Sermepa
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
            NombreComercioTextBox.Text = SettingManager.GetSettingValue("PaymentMethod.Sermepa.NombreComercio");
            TitularTextBox.Text = SettingManager.GetSettingValue("PaymentMethod.Sermepa.Titular");
            ProductoTextBox.Text = SettingManager.GetSettingValue("PaymentMethod.Sermepa.Producto");
            FUCTextBox.Text = SettingManager.GetSettingValue("PaymentMethod.Sermepa.FUC");
            TerminalTextBox.Text = SettingManager.GetSettingValue("PaymentMethod.Sermepa.Terminal");
            MonedaTextBox.Text = SettingManager.GetSettingValue("PaymentMethod.Sermepa.Moneda");
            ClaveRealTextBox.Text = SettingManager.GetSettingValue("PaymentMethod.Sermepa.ClaveReal");
            ClavePruebasTextBox.Text = SettingManager.GetSettingValue("PaymentMethod.Sermepa.ClavePruebas");
            PruebasCheckBox.Checked = SettingManager.GetSettingValueBoolean("PaymentMethod.Sermepa.Pruebas");
            txtAdditionalFee.Value = SettingManager.GetSettingValueDecimalNative("PaymentMethod.Sermepa.AdditionalFee", decimal.Zero);
        }

        public void Save()
        {
            SettingManager.SetParam("PaymentMethod.Sermepa.NombreComercio", NombreComercioTextBox.Text);
            SettingManager.SetParam("PaymentMethod.Sermepa.Titular", TitularTextBox.Text);
            SettingManager.SetParam("PaymentMethod.Sermepa.Producto", ProductoTextBox.Text);
            SettingManager.SetParam("PaymentMethod.Sermepa.FUC", FUCTextBox.Text);
            SettingManager.SetParam("PaymentMethod.Sermepa.Terminal", TerminalTextBox.Text);
            SettingManager.SetParam("PaymentMethod.Sermepa.Moneda", MonedaTextBox.Text);
            SettingManager.SetParam("PaymentMethod.Sermepa.ClaveReal", ClaveRealTextBox.Text);
            SettingManager.SetParam("PaymentMethod.Sermepa.ClavePruebas", ClavePruebasTextBox.Text);
            SettingManager.SetParam("PaymentMethod.Sermepa.Pruebas", PruebasCheckBox.Checked.ToString());
            SettingManager.SetParamNative("PaymentMethod.Sermepa.AdditionalFee", txtAdditionalFee.Value);
        }
    }
}

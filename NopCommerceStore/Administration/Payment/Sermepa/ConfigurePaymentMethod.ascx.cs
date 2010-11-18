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
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

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
            NombreComercioTextBox.Text = this.SettingManager.GetSettingValue("PaymentMethod.Sermepa.NombreComercio");
            TitularTextBox.Text = this.SettingManager.GetSettingValue("PaymentMethod.Sermepa.Titular");
            ProductoTextBox.Text = this.SettingManager.GetSettingValue("PaymentMethod.Sermepa.Producto");
            FUCTextBox.Text = this.SettingManager.GetSettingValue("PaymentMethod.Sermepa.FUC");
            TerminalTextBox.Text = this.SettingManager.GetSettingValue("PaymentMethod.Sermepa.Terminal");
            MonedaTextBox.Text = this.SettingManager.GetSettingValue("PaymentMethod.Sermepa.Moneda");
            ClaveRealTextBox.Text = this.SettingManager.GetSettingValue("PaymentMethod.Sermepa.ClaveReal");
            ClavePruebasTextBox.Text = this.SettingManager.GetSettingValue("PaymentMethod.Sermepa.ClavePruebas");
            PruebasCheckBox.Checked = this.SettingManager.GetSettingValueBoolean("PaymentMethod.Sermepa.Pruebas");
            txtAdditionalFee.Value = this.SettingManager.GetSettingValueDecimalNative("PaymentMethod.Sermepa.AdditionalFee", decimal.Zero);
        }

        public void Save()
        {
            this.SettingManager.SetParam("PaymentMethod.Sermepa.NombreComercio", NombreComercioTextBox.Text);
            this.SettingManager.SetParam("PaymentMethod.Sermepa.Titular", TitularTextBox.Text);
            this.SettingManager.SetParam("PaymentMethod.Sermepa.Producto", ProductoTextBox.Text);
            this.SettingManager.SetParam("PaymentMethod.Sermepa.FUC", FUCTextBox.Text);
            this.SettingManager.SetParam("PaymentMethod.Sermepa.Terminal", TerminalTextBox.Text);
            this.SettingManager.SetParam("PaymentMethod.Sermepa.Moneda", MonedaTextBox.Text);
            this.SettingManager.SetParam("PaymentMethod.Sermepa.ClaveReal", ClaveRealTextBox.Text);
            this.SettingManager.SetParam("PaymentMethod.Sermepa.ClavePruebas", ClavePruebasTextBox.Text);
            this.SettingManager.SetParam("PaymentMethod.Sermepa.Pruebas", PruebasCheckBox.Checked.ToString());
            this.SettingManager.SetParamNative("PaymentMethod.Sermepa.AdditionalFee", txtAdditionalFee.Value);
        }
    }
}

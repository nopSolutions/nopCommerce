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
            NombreComercioTextBox.Text = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.NombreComercio");
            TitularTextBox.Text = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.Titular");
            ProductoTextBox.Text = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.Producto");
            FUCTextBox.Text = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.FUC");
            TerminalTextBox.Text = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.Terminal");
            MonedaTextBox.Text = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.Moneda");
            ClaveRealTextBox.Text = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.ClaveReal");
            ClavePruebasTextBox.Text = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.ClavePruebas");
            PruebasCheckBox.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.Sermepa.Pruebas");
            txtAdditionalFee.Value = IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.Sermepa.AdditionalFee", decimal.Zero);
        }

        public void Save()
        {
            IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Sermepa.NombreComercio", NombreComercioTextBox.Text);
            IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Sermepa.Titular", TitularTextBox.Text);
            IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Sermepa.Producto", ProductoTextBox.Text);
            IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Sermepa.FUC", FUCTextBox.Text);
            IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Sermepa.Terminal", TerminalTextBox.Text);
            IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Sermepa.Moneda", MonedaTextBox.Text);
            IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Sermepa.ClaveReal", ClaveRealTextBox.Text);
            IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Sermepa.ClavePruebas", ClavePruebasTextBox.Text);
            IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Sermepa.Pruebas", PruebasCheckBox.Checked.ToString());
            IoC.Resolve<ISettingManager>().SetParamNative("PaymentMethod.Sermepa.AdditionalFee", txtAdditionalFee.Value);
        }
    }
}

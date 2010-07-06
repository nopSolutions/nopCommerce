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
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.Web.Templates.Shipping;

namespace NopSolutions.NopCommerce.Web.Administration.Shipping.USPSConfigure
{
    public partial class ConfigureShipping : BaseNopAdministrationUserControl, IConfigureShippingRateComputationMethodModule
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
            txtURL.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.USPS.URL");
            txtUsername.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.USPS.Username");
            txtPassword.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.USPS.Password");
            txtAdditionalHandlingCharge.Value = SettingManager.GetSettingValueDecimalNative("ShippingRateComputationMethod.USPS.AdditionalHandlingCharge");
            txtShippedFromZipPostalCode.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.USPS.DefaultShippedFromZipPostalCode");
        }

        public void Save()
        {
            SettingManager.SetParam("ShippingRateComputationMethod.USPS.URL", txtURL.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.USPS.Username", txtUsername.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.USPS.Password", txtPassword.Text);
            SettingManager.SetParamNative("ShippingRateComputationMethod.USPS.AdditionalHandlingCharge", txtAdditionalHandlingCharge.Value);
            SettingManager.SetParam("ShippingRateComputationMethod.USPS.DefaultShippedFromZipPostalCode", txtShippedFromZipPostalCode.Text);
        }
    }
}

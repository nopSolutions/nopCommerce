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

namespace NopSolutions.NopCommerce.Web.Administration.Shipping.FedexConfigure
{
    /// <summary>
    /// Configure shipping
    /// </summary>
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
            txtURL.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.FedEx.URL");
            txtKey.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.FedEx.Key");
            txtPassword.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.FedEx.Password");
            txtAccountNumber.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.FedEx.AccountNumber");
            txtMeterNumber.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.FedEx.MeterNumber");
            cbUseResidentialRates.Checked = SettingManager.GetSettingValueBoolean("ShippingRateComputationMethod.FedEx.UseResidentialRates", false);
            txtShippingOriginStreet.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.FedEx.ShippingOrigin.Street");
            txtShippingOriginCity.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.FedEx.ShippingOrigin.City");
            txtShippingOriginStateOrProvinceCode.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.FedEx.ShippingOrigin.StateOrProvinceCode");
            txtShippingOriginPostalCode.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.FedEx.ShippingOrigin.PostalCode");
            txtShippingOriginCountryCode.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.FedEx.ShippingOrigin.CountryCode");
        }

        public void Save()
        {
            SettingManager.SetParam("ShippingRateComputationMethod.FedEx.URL", txtURL.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.FedEx.Key", txtKey.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.FedEx.Password", txtPassword.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.FedEx.AccountNumber", txtAccountNumber.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.FedEx.MeterNumber", txtMeterNumber.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.FedEx.UseResidentialRates", cbUseResidentialRates.Checked.ToString());
            SettingManager.SetParam("ShippingRateComputationMethod.FedEx.ShippingOrigin.Street", txtShippingOriginStreet.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.FedEx.ShippingOrigin.City", txtShippingOriginCity.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.FedEx.ShippingOrigin.StateOrProvinceCode", txtShippingOriginStateOrProvinceCode.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.FedEx.ShippingOrigin.PostalCode", txtShippingOriginPostalCode.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.FedEx.ShippingOrigin.CountryCode", txtShippingOriginCountryCode.Text);
        }
    }
}
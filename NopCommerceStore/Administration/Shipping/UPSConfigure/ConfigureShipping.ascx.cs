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
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Shipping.Methods.UPS;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.Web.Templates.Shipping;

namespace NopSolutions.NopCommerce.Web.Administration.Shipping.UPSConfigure
{
    public partial class ConfigureShipping : BaseNopAdministrationUserControl, IConfigureShippingRateComputationMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillDropDowns();
                BindData();
            }
        }

        private void FillDropDowns()
        {
            this.ddlUPSCustomerClassification.Items.Clear();
            string[] customerClassifications = Enum.GetNames(typeof(UPSCustomerClassification));
            foreach (string cc in customerClassifications)
            {
                ListItem ddlItem1 = new ListItem(CommonHelper.ConvertEnum(cc), cc);
                this.ddlUPSCustomerClassification.Items.Add(ddlItem1);
            }

            this.ddlUPSPickupType.Items.Clear();
            string[] pickupTypies = Enum.GetNames(typeof(UPSPickupType));
            foreach (string pt in pickupTypies)
            {
                ListItem ddlItem1 = new ListItem(CommonHelper.ConvertEnum(pt), pt);
                this.ddlUPSPickupType.Items.Add(ddlItem1);
            }

            this.ddlUPSPackagingType.Items.Clear();
            string[] packagingTypies = Enum.GetNames(typeof(UPSPackagingType));
            foreach (string pt in packagingTypies)
            {
                ListItem ddlItem1 = new ListItem(CommonHelper.ConvertEnum(pt), pt);
                this.ddlUPSPackagingType.Items.Add(ddlItem1);
            }

            this.ddlShippedFromCountry.Items.Clear();
            var countries = CountryManager.GetAllCountries();
            foreach (Country country in countries)
            {
                ListItem ddlItem1 = new ListItem(country.Name, country.CountryId.ToString());
                this.ddlShippedFromCountry.Items.Add(ddlItem1);
            }
        }

        private void BindData()
        {
            txtURL.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.URL");
            txtAccessKey.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.AccessKey");
            txtUsername.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.Username");
            txtPassword.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.Password");
            txtAdditionalHandlingCharge.Value = SettingManager.GetSettingValueDecimalNative("ShippingRateComputationMethod.UPS.AdditionalHandlingCharge");

            int defaultShippedFromCountryId = SettingManager.GetSettingValueInteger("ShippingRateComputationMethod.UPS.DefaultShippedFromCountryId");
            CommonHelper.SelectListItem(ddlShippedFromCountry, defaultShippedFromCountryId);
            txtShippedFromZipPostalCode.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.DefaultShippedFromZipPostalCode");


            string customerClassificationStr = SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.CustomerClassification");
            if (!String.IsNullOrEmpty(customerClassificationStr))
            {
                UPSCustomerClassification customerClassification = (UPSCustomerClassification)Enum.Parse(typeof(UPSCustomerClassification), customerClassificationStr);
                CommonHelper.SelectListItem(ddlUPSCustomerClassification, customerClassification.ToString());
            }

            string pickupTypeStr = SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.PickupType");
            if (!String.IsNullOrEmpty(pickupTypeStr))
            {
                UPSPickupType pickupType = (UPSPickupType)Enum.Parse(typeof(UPSPickupType), pickupTypeStr);
                CommonHelper.SelectListItem(ddlUPSPickupType, pickupType.ToString());
            }

            string packagingTypeStr = SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.PackagingType");
            if (!String.IsNullOrEmpty(packagingTypeStr))
            {
                UPSPackagingType packagingType = (UPSPackagingType)Enum.Parse(typeof(UPSPackagingType), packagingTypeStr);
                CommonHelper.SelectListItem(ddlUPSPackagingType, packagingType.ToString());
            }
        }

        public void Save()
        {
            UPSCustomerClassification customerClassification = (UPSCustomerClassification)Enum.Parse(typeof(UPSCustomerClassification), ddlUPSCustomerClassification.SelectedItem.Value);
            UPSPickupType pickupType = (UPSPickupType)Enum.Parse(typeof(UPSPickupType), ddlUPSPickupType.SelectedItem.Value);
            UPSPackagingType packagingType = (UPSPackagingType)Enum.Parse(typeof(UPSPackagingType), ddlUPSPackagingType.SelectedItem.Value);

            SettingManager.SetParam("ShippingRateComputationMethod.UPS.URL", txtURL.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.UPS.AccessKey", txtAccessKey.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.UPS.Username", txtUsername.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.UPS.Password", txtPassword.Text);
            SettingManager.SetParamNative("ShippingRateComputationMethod.UPS.AdditionalHandlingCharge", txtAdditionalHandlingCharge.Value);
            int defaultShippedFromCountryId = int.Parse(this.ddlShippedFromCountry.SelectedItem.Value);
            SettingManager.SetParam("ShippingRateComputationMethod.UPS.DefaultShippedFromCountryId", defaultShippedFromCountryId.ToString());
            SettingManager.SetParam("ShippingRateComputationMethod.UPS.DefaultShippedFromZipPostalCode", txtShippedFromZipPostalCode.Text);

            SettingManager.SetParam("ShippingRateComputationMethod.UPS.CustomerClassification", customerClassification.ToString());
            SettingManager.SetParam("ShippingRateComputationMethod.UPS.PickupType", pickupType.ToString());
            SettingManager.SetParam("ShippingRateComputationMethod.UPS.PackagingType", packagingType.ToString());
        }
    }
}

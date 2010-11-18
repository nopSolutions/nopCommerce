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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Shipping.Methods.UPS;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.Web.Templates.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

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
            var countries = this.CountryService.GetAllCountries();
            foreach (Country country in countries)
            {
                ListItem ddlItem1 = new ListItem(country.Name, country.CountryId.ToString());
                this.ddlShippedFromCountry.Items.Add(ddlItem1);
            }
        }

        private void BindData()
        {
            txtURL.Text = this.SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.URL");
            txtAccessKey.Text = this.SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.AccessKey");
            txtUsername.Text = this.SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.Username");
            txtPassword.Text = this.SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.Password");
            txtAdditionalHandlingCharge.Value = this.SettingManager.GetSettingValueDecimalNative("ShippingRateComputationMethod.UPS.AdditionalHandlingCharge");

            int defaultShippedFromCountryId = this.SettingManager.GetSettingValueInteger("ShippingRateComputationMethod.UPS.DefaultShippedFromCountryId");
            CommonHelper.SelectListItem(ddlShippedFromCountry, defaultShippedFromCountryId);
            txtShippedFromZipPostalCode.Text = this.SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.DefaultShippedFromZipPostalCode");


            string customerClassificationStr = this.SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.CustomerClassification");
            if (!String.IsNullOrEmpty(customerClassificationStr))
            {
                UPSCustomerClassification customerClassification = (UPSCustomerClassification)Enum.Parse(typeof(UPSCustomerClassification), customerClassificationStr);
                CommonHelper.SelectListItem(ddlUPSCustomerClassification, customerClassification.ToString());
            }

            string pickupTypeStr = this.SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.PickupType");
            if (!String.IsNullOrEmpty(pickupTypeStr))
            {
                UPSPickupType pickupType = (UPSPickupType)Enum.Parse(typeof(UPSPickupType), pickupTypeStr);
                CommonHelper.SelectListItem(ddlUPSPickupType, pickupType.ToString());
            }

            string packagingTypeStr = this.SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.PackagingType");
            if (!String.IsNullOrEmpty(packagingTypeStr))
            {
                UPSPackagingType packagingType = (UPSPackagingType)Enum.Parse(typeof(UPSPackagingType), packagingTypeStr);
                CommonHelper.SelectListItem(ddlUPSPackagingType, packagingType.ToString());
            }
            
            string carrierServicesOffered = this.SettingManager.GetSettingValue("ShippingRateComputationMethod.UPS.CarrierServicesOffered", string.Empty);
            var services = new UPSServices();
            // Load all service names
            if (carrierServicesOffered.Length == 0)
            {
                foreach (string service in services.Services)
                {
                    this.cblCarrierServicesOffered.Items.Add(service);
                }
            }
            else
            {
                // Load and select previously selected services
                foreach (string service in services.Services)
                {
                    ListItem cblItem = new ListItem(service);
                    string serviceId = UPSServices.GetServiceId(service);

                    if (!String.IsNullOrEmpty(serviceId) && !String.IsNullOrEmpty(carrierServicesOffered))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        serviceId = String.Format("[{0}]", serviceId);
                        if (carrierServicesOffered.Contains(serviceId) == true)
                        {
                            cblItem.Selected = true;
                        }
                    }
                    this.cblCarrierServicesOffered.Items.Add(cblItem);
                }
            }
        }

        public void Save()
        {
            UPSCustomerClassification customerClassification = (UPSCustomerClassification)Enum.Parse(typeof(UPSCustomerClassification), ddlUPSCustomerClassification.SelectedItem.Value);
            UPSPickupType pickupType = (UPSPickupType)Enum.Parse(typeof(UPSPickupType), ddlUPSPickupType.SelectedItem.Value);
            UPSPackagingType packagingType = (UPSPackagingType)Enum.Parse(typeof(UPSPackagingType), ddlUPSPackagingType.SelectedItem.Value);

            this.SettingManager.SetParam("ShippingRateComputationMethod.UPS.URL", txtURL.Text);
            this.SettingManager.SetParam("ShippingRateComputationMethod.UPS.AccessKey", txtAccessKey.Text);
            this.SettingManager.SetParam("ShippingRateComputationMethod.UPS.Username", txtUsername.Text);
            this.SettingManager.SetParam("ShippingRateComputationMethod.UPS.Password", txtPassword.Text);
            this.SettingManager.SetParamNative("ShippingRateComputationMethod.UPS.AdditionalHandlingCharge", txtAdditionalHandlingCharge.Value);
            int defaultShippedFromCountryId = int.Parse(this.ddlShippedFromCountry.SelectedItem.Value);
            this.SettingManager.SetParam("ShippingRateComputationMethod.UPS.DefaultShippedFromCountryId", defaultShippedFromCountryId.ToString());
            this.SettingManager.SetParam("ShippingRateComputationMethod.UPS.DefaultShippedFromZipPostalCode", txtShippedFromZipPostalCode.Text);

            this.SettingManager.SetParam("ShippingRateComputationMethod.UPS.CustomerClassification", customerClassification.ToString());
            this.SettingManager.SetParam("ShippingRateComputationMethod.UPS.PickupType", pickupType.ToString());
            this.SettingManager.SetParam("ShippingRateComputationMethod.UPS.PackagingType", packagingType.ToString());


            var carrierServicesOffered = new StringBuilder();
            int carrierServicesCount = 0;
            foreach (ListItem li in cblCarrierServicesOffered.Items)
            {
                if (li.Selected == true)
                {
                    string serviceId = UPSServices.GetServiceId(li.Text);
                    if (!String.IsNullOrEmpty(serviceId))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        carrierServicesOffered.AppendFormat("[{0}]:", serviceId);
                    }
                    carrierServicesCount++;
                }
            }
            // Add default options if no services were selected (Ground, 3 Day Select, Standard, and Worldwide Expedited)
            if (carrierServicesCount == 0)
            {
                this.SettingManager.SetParam("ShippingRateComputationMethod.UPS.CarrierServicesOffered", "[03]:[12]:[11]:[08]:");
            }
            else
            {
                this.SettingManager.SetParam("ShippingRateComputationMethod.UPS.CarrierServicesOffered", carrierServicesOffered.ToString());
            }
        }
    }
}

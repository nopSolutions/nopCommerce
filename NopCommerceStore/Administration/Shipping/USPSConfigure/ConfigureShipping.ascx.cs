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
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Shipping.Methods.USPS;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.Web.Templates.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

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
            txtURL.Text = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.USPS.URL");
            txtUsername.Text = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.USPS.Username");
            txtPassword.Text = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.USPS.Password");
            txtAdditionalHandlingCharge.Value = IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("ShippingRateComputationMethod.USPS.AdditionalHandlingCharge");
            txtShippedFromZipPostalCode.Text = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.USPS.DefaultShippedFromZipPostalCode");


            string carrierServicesOfferedDomestic = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.USPS.CarrierServicesOfferedDomestic", string.Empty);
            var services = new USPSServices();
            // Load Domestic service names
            if (carrierServicesOfferedDomestic.Length == 0)
            {
                foreach (string service in services.DomesticServices)
                {
                    this.cblCarrierServicesOfferedDomestic.Items.Add(service);
                }
            }
            else
            {
                foreach (string service in services.DomesticServices)
                {
                    ListItem cblItem = new ListItem(service);
                    string serviceId = USPSServices.GetServiceIdDomestic(service);

                    if (!String.IsNullOrEmpty(serviceId) && !String.IsNullOrEmpty(carrierServicesOfferedDomestic))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        serviceId = String.Format("[{0}]", serviceId);
                        if (carrierServicesOfferedDomestic.Contains(serviceId) == true)
                        {
                            cblItem.Selected = true;
                        }
                    }
                    this.cblCarrierServicesOfferedDomestic.Items.Add(cblItem);
                }
            }

            string carrierServicesOfferedInternational = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.USPS.CarrierServicesOfferedInternational", string.Empty);
            // Load International service names
            if (carrierServicesOfferedInternational.Length == 0)
            {
                foreach (string service in services.InternationalServices)
                {
                    this.cblCarrierServicesOfferedInternational.Items.Add(service);
                }
            }
            else
            {
                foreach (string service in services.InternationalServices)
                {
                    ListItem cblItem = new ListItem(service);
                    string serviceId = USPSServices.GetServiceIdInternational(service);

                    if (!String.IsNullOrEmpty(serviceId) && !String.IsNullOrEmpty(carrierServicesOfferedInternational))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        serviceId = String.Format("[{0}]", serviceId);
                        if (carrierServicesOfferedInternational.Contains(serviceId) == true)
                        {
                            cblItem.Selected = true;
                        }
                    }
                    this.cblCarrierServicesOfferedInternational.Items.Add(cblItem);
                }
            }
        }

        public void Save()
        {
            IoCFactory.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.USPS.URL", txtURL.Text);
            IoCFactory.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.USPS.Username", txtUsername.Text);
            IoCFactory.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.USPS.Password", txtPassword.Text);
            IoCFactory.Resolve<ISettingManager>().SetParamNative("ShippingRateComputationMethod.USPS.AdditionalHandlingCharge", txtAdditionalHandlingCharge.Value);
            IoCFactory.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.USPS.DefaultShippedFromZipPostalCode", txtShippedFromZipPostalCode.Text);

            // Save selected Domestic services
            var carrierServicesOfferedDomestic = new StringBuilder();
            int carrierServicesDomesticSelectedCount = 0;
            foreach (ListItem li in cblCarrierServicesOfferedDomestic.Items)
            {
                if (li.Selected == true)
                {
                    string serviceId = USPSServices.GetServiceIdDomestic(li.Text);
                    if (!String.IsNullOrEmpty(serviceId))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        carrierServicesOfferedDomestic.AppendFormat("[{0}]:", serviceId);
                    }
                    carrierServicesDomesticSelectedCount++;
                }
            }
            // Add default options if no services were selected (Priority, Express, and Parcel Post)
            if (carrierServicesDomesticSelectedCount == 0)
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.USPS.CarrierServicesOfferedDomestic", "[1]:[3]:[4]:");
            }
            else
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.USPS.CarrierServicesOfferedDomestic", carrierServicesOfferedDomestic.ToString());
            }

            // Save selected International services
            var carrierServicesOfferedInternational = new StringBuilder();
            int carrierServicesInternationalSelectedCount = 0;
            foreach (ListItem li in cblCarrierServicesOfferedInternational.Items)
            {
                if (li.Selected == true)
                {
                    string serviceId = USPSServices.GetServiceIdInternational(li.Text);
                    if (!String.IsNullOrEmpty(serviceId))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        carrierServicesOfferedInternational.AppendFormat("[{0}]:", serviceId);
                    }
                    carrierServicesInternationalSelectedCount++;
                }
            }
            // Add default options if no services were selected (Priority Mail International, First-Class Mail International Package, and Express Mail International)
            if (carrierServicesInternationalSelectedCount == 0)
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.USPS.CarrierServicesOfferedInternational", "[2]:[15]:[1]:");
            }
            else
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.USPS.CarrierServicesOfferedInternational", carrierServicesOfferedInternational.ToString());
            }
        }
    }
}

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
using NopSolutions.NopCommerce.Shipping.Methods.FedEx;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.Web.Templates.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

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
            txtURL.Text = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.URL");
            txtKey.Text = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.Key");
            txtPassword.Text = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.Password");
            txtAccountNumber.Text = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.AccountNumber");
            txtMeterNumber.Text = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.MeterNumber");
            cbUseResidentialRates.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("ShippingRateComputationMethod.FedEx.UseResidentialRates", false);
            cbApplyDiscounts.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("ShippingRateComputationMethod.FedEx.ApplyDiscounts", false);
            txtAdditionalFee.Value = IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("ShippingRateComputationMethod.FedEx.AdditionalFee");
            txtShippingOriginStreet.Text = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.ShippingOrigin.Street");
            txtShippingOriginCity.Text = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.ShippingOrigin.City");
            txtShippingOriginStateOrProvinceCode.Text = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.ShippingOrigin.StateOrProvinceCode");
            txtShippingOriginPostalCode.Text = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.ShippingOrigin.PostalCode");
            txtShippingOriginCountryCode.Text = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.ShippingOrigin.CountryCode");


            // Get the selected offered services from the database
            string carrierServicesOffered = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.CarrierServicesOffered");
            var services = new FedExServices();
            // Load default options
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
                    var cblItem = new ListItem(service);
                    string serviceId = FedExServices.GetServiceId(service);

                    if (!String.IsNullOrEmpty(serviceId) && !String.IsNullOrEmpty(carrierServicesOffered))
                    {
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
            IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.FedEx.URL", txtURL.Text);
            IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.FedEx.Key", txtKey.Text);
            IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.FedEx.Password", txtPassword.Text);
            IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.FedEx.AccountNumber", txtAccountNumber.Text);
            IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.FedEx.MeterNumber", txtMeterNumber.Text);
            IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.FedEx.UseResidentialRates", cbUseResidentialRates.Checked.ToString());
            IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.FedEx.ApplyDiscounts", cbApplyDiscounts.Checked.ToString());
            IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.FedEx.AdditionalFee", txtAdditionalFee.Value.ToString());
            IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.FedEx.ShippingOrigin.Street", txtShippingOriginStreet.Text);
            IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.FedEx.ShippingOrigin.City", txtShippingOriginCity.Text);
            IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.FedEx.ShippingOrigin.StateOrProvinceCode", txtShippingOriginStateOrProvinceCode.Text);
            IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.FedEx.ShippingOrigin.PostalCode", txtShippingOriginPostalCode.Text);
            IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.FedEx.ShippingOrigin.CountryCode", txtShippingOriginCountryCode.Text);

            var carrierServicesOffered = new StringBuilder();
            int carrierServicesSelectedCount = 0;
            foreach (ListItem li in cblCarrierServicesOffered.Items)
            {
                if (li.Selected == true)
                {
                    string serviceId = FedExServices.GetServiceId(li.Text);
                    if (serviceId.Equals("") == false)
                    {
                        carrierServicesOffered.Append(serviceId);
                        carrierServicesOffered.Append(":");
                    }
                    carrierServicesSelectedCount++;
                }
            }
            // Add default options if no services were selected
            if (carrierServicesSelectedCount == 0)
            {
                IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.FedEx.CarrierServicesOffered", "FEDEX_2_DAY:PRIORITY_OVERNIGHT:FEDEX_GROUND:GROUND_HOME_DELIVERY:INTERNATIONAL_ECONOMY");
            }
            else
            {
                IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.FedEx.CarrierServicesOffered", carrierServicesOffered.ToString());
            }
        }
    }
}
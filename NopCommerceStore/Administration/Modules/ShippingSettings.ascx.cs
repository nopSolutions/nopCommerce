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
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ShippingSettingsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }


        protected void FillCountryDropDowns()
        {
            this.ddlShippingOriginCountry.Items.Clear();
            ListItem selectCountryItem = new ListItem(GetLocaleResourceString("Admin.ShippingSettings.OriginCountry.SelectCountry"), "0");
            this.ddlShippingOriginCountry.Items.Add(selectCountryItem);
            var countryCollection = CountryManager.GetAllCountries();
            foreach (Country country in countryCollection)
            {
                ListItem ddlCountryItem2 = new ListItem(country.Name, country.CountryId.ToString());
                this.ddlShippingOriginCountry.Items.Add(ddlCountryItem2);
            }
        }

        protected void FillStateProvinceDropDowns()
        {
            this.ddlShippingOriginStateProvince.Items.Clear();
            int countryId = int.Parse(this.ddlShippingOriginCountry.SelectedItem.Value);

            var stateProvinceCollection = StateProvinceManager.GetStateProvincesByCountryId(countryId);
            foreach (StateProvince stateProvince in stateProvinceCollection)
            {
                ListItem ddlStateProviceItem2 = new ListItem(stateProvince.Name, stateProvince.StateProvinceId.ToString());
                this.ddlShippingOriginStateProvince.Items.Add(ddlStateProviceItem2);
            }
            if (stateProvinceCollection.Count == 0)
            {
                ListItem ddlStateProvinceItem = new ListItem(GetLocaleResourceString("Admin.Common.State.Other"), "0");
                this.ddlShippingOriginStateProvince.Items.Add(ddlStateProvinceItem);
            }
        }

        private void BindData()
        {
            cbEstimateShippingEnabled.Checked = SettingManager.GetSettingValueBoolean("Shipping.EstimateShipping.Enabled");
            cbFreeShippingOverX.Checked = SettingManager.GetSettingValueBoolean("Shipping.FreeShippingOverX.Enabled");
            txtFreeShippingOverX.Value = SettingManager.GetSettingValueDecimalNative("Shipping.FreeShippingOverX.Value");

            Address shippingOriginAddress = ShippingManager.ShippingOrigin;
            this.FillCountryDropDowns();
            CommonHelper.SelectListItem(this.ddlShippingOriginCountry, shippingOriginAddress.CountryId);
            this.FillStateProvinceDropDowns();
            CommonHelper.SelectListItem(this.ddlShippingOriginStateProvince, shippingOriginAddress.StateProvinceId);
            txtShippingOriginZipPostalCode.Text = shippingOriginAddress.ZipPostalCode;
        }

        protected void ddlShippingOriginCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStateProvinceDropDowns();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    SettingManager.SetParam("Shipping.EstimateShipping.Enabled", cbEstimateShippingEnabled.Checked.ToString());
                    SettingManager.SetParam("Shipping.FreeShippingOverX.Enabled", cbFreeShippingOverX.Checked.ToString());
                    SettingManager.SetParamNative("Shipping.FreeShippingOverX.Value", txtFreeShippingOverX.Value);

                    Address shippingOriginAddress = new Address();
                    shippingOriginAddress.CountryId = int.Parse(this.ddlShippingOriginCountry.SelectedItem.Value);
                    shippingOriginAddress.StateProvinceId = int.Parse(this.ddlShippingOriginStateProvince.SelectedItem.Value);
                    shippingOriginAddress.ZipPostalCode = txtShippingOriginZipPostalCode.Text;
                    ShippingManager.ShippingOrigin = shippingOriginAddress;
                    
                    CustomerActivityManager.InsertActivity(
                        "EditShippingSettings",
                        GetLocaleResourceString("ActivityLog.EditShippingSettings"));

                    Response.Redirect("ShippingSettings.aspx");
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

    }
}
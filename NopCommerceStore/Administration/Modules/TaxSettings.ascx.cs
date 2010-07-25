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
using System.Linq;
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
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class TaxSettingsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillDropDowns();
                BindData();
            }
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();
            BindJQueryIdTabs();

            this.cbAllowCustomersToSelectTaxDisplayType.Attributes.Add("onclick", "toggleTaxDisplayType();");
            this.cbShippingIsTaxable.Attributes.Add("onclick", "toggleShipping();");
            this.cbPaymentMethodAdditionalFeeIsTaxable.Attributes.Add("onclick", "togglePayment();");
            this.cbEUVatEnabled.Attributes.Add("onclick", "toggleEUVAT();");

            base.OnPreRender(e);
        }

        protected void FillDropDowns()
        {
            CommonHelper.FillDropDownWithEnum(this.ddlTaxDisplayType, typeof(TaxDisplayTypeEnum));
            CommonHelper.FillDropDownWithEnum(this.ddlTaxBasedOn, typeof(TaxBasedOnEnum));

            var taxCategoryCollection = TaxCategoryManager.GetAllTaxCategories();

            this.ddlShippingTaxClass.Items.Clear();
            ListItem itemShippingTaxCategory = new ListItem("---", "0");
            this.ddlShippingTaxClass.Items.Add(itemShippingTaxCategory);
            foreach (TaxCategory taxCategory in taxCategoryCollection)
            {
                ListItem item2 = new ListItem(taxCategory.Name, taxCategory.TaxCategoryId.ToString());
                this.ddlShippingTaxClass.Items.Add(item2);
            }

            this.ddlPaymentMethodAdditionalFeeTaxClass.Items.Clear();
            ListItem itemPaymentMethodAdditionalFeeTaxCategory = new ListItem("---", "0");
            this.ddlPaymentMethodAdditionalFeeTaxClass.Items.Add(itemPaymentMethodAdditionalFeeTaxCategory);
            foreach (TaxCategory taxCategory in taxCategoryCollection)
            {
                ListItem item2 = new ListItem(taxCategory.Name, taxCategory.TaxCategoryId.ToString());
                this.ddlPaymentMethodAdditionalFeeTaxClass.Items.Add(item2);
            }
        }

        protected void FillCountryDropDowns()
        {
            var countries = CountryManager.GetAllCountries();

            this.ddlTaxDefaultCountry.Items.Clear();
            ListItem noCountryItem1 = new ListItem(GetLocaleResourceString("Admin.TaxSettings.TaxDefaultCountry.SelectCountry"), "0");
            this.ddlTaxDefaultCountry.Items.Add(noCountryItem1);
            foreach (Country country in countries)
            {
                ListItem ddlCountryItem2 = new ListItem(country.Name, country.CountryId.ToString());
                this.ddlTaxDefaultCountry.Items.Add(ddlCountryItem2);
            }
        }

        protected void FillStateProvinceDropDowns()
        {
            this.ddlTaxDefaultStateProvince.Items.Clear();
            int countryId = int.Parse(this.ddlTaxDefaultCountry.SelectedItem.Value);

            var stateProvinceCollection = StateProvinceManager.GetStateProvincesByCountryId(countryId);
            foreach (StateProvince stateProvince in stateProvinceCollection)
            {
                ListItem ddlStateProviceItem2 = new ListItem(stateProvince.Name, stateProvince.StateProvinceId.ToString());
                this.ddlTaxDefaultStateProvince.Items.Add(ddlStateProviceItem2);
            }
            if (stateProvinceCollection.Count == 0)
            {
                ListItem ddlStateProvinceItem = new ListItem(GetLocaleResourceString("Admin.Common.State.Other"), "0");
                this.ddlTaxDefaultStateProvince.Items.Add(ddlStateProvinceItem);
            }
        }

        protected void FillEUVATCountryDropDowns()
        {
            var countries = CountryManager.GetAllCountries().Where(country => country.SubjectToVAT).ToList();

            this.ddlEUVatShopCountry.Items.Clear();
            ListItem noCountryItem2 = new ListItem(GetLocaleResourceString("Admin.TaxSettings.EUVatShopCountry.SelectCountry"), "0");
            this.ddlEUVatShopCountry.Items.Add(noCountryItem2);
            foreach (Country country in countries)
            {
                ListItem ddlCountryItem2 = new ListItem(country.Name, country.CountryId.ToString());
                this.ddlEUVatShopCountry.Items.Add(ddlCountryItem2);
            }
        }

        private void BindData()
        {
            //standard settings
            cbPricesIncludeTax.Checked = TaxManager.PricesIncludeTax;
            cbAllowCustomersToSelectTaxDisplayType.Checked = TaxManager.AllowCustomersToSelectTaxDisplayType;
            CommonHelper.SelectListItem(this.ddlTaxDisplayType, (int)TaxManager.TaxDisplayType);
            cbDisplayTaxSuffix.Checked = TaxManager.DisplayTaxSuffix;
            cbDisplayTaxRates.Checked = TaxManager.DisplayTaxRates;
            cbHideZeroTax.Checked = TaxManager.HideZeroTax;
            cbHideTaxInOrderSummary.Checked = TaxManager.HideTaxInOrderSummary;
            CommonHelper.SelectListItem(this.ddlTaxBasedOn, (int)TaxManager.TaxBasedOn);

            //tax address
            Address defaultTaxAddress = TaxManager.DefaultTaxAddress;
            this.FillCountryDropDowns();
            CommonHelper.SelectListItem(this.ddlTaxDefaultCountry, defaultTaxAddress.CountryId);
            this.FillStateProvinceDropDowns();
            CommonHelper.SelectListItem(this.ddlTaxDefaultStateProvince, defaultTaxAddress.StateProvinceId);
            txtTaxDefaultZipPostalCode.Text = defaultTaxAddress.ZipPostalCode;

            //shipping
            cbShippingIsTaxable.Checked = TaxManager.ShippingIsTaxable;
            cbShippingPriceIncludesTax.Checked = TaxManager.ShippingPriceIncludesTax;
            CommonHelper.SelectListItem(this.ddlShippingTaxClass, TaxManager.ShippingTaxClassId);

            //additional payment fee
            cbPaymentMethodAdditionalFeeIsTaxable.Checked = TaxManager.PaymentMethodAdditionalFeeIsTaxable;
            cbPaymentMethodAdditionalFeeIncludesTax.Checked = TaxManager.PaymentMethodAdditionalFeeIncludesTax;
            CommonHelper.SelectListItem(this.ddlPaymentMethodAdditionalFeeTaxClass, TaxManager.PaymentMethodAdditionalFeeTaxClassId);

            //EU VAT support
            cbEUVatEnabled.Checked = TaxManager.EUVatEnabled;
            this.FillEUVATCountryDropDowns();
            CommonHelper.SelectListItem(this.ddlEUVatShopCountry, TaxManager.EUVatShopCountryId);
            cbEUVatAllowVATExemption.Checked = TaxManager.EUVatAllowVATExemption;
            cbEUVatUseWebService.Checked = TaxManager.EUVatUseWebService;
            cbEUVatEmailAdminWhenNewVATSubmitted.Checked = TaxManager.EUVatEmailAdminWhenNewVATSubmitted;
        }

        protected void ddlTaxDefaultCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStateProvinceDropDowns();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    //standard settings
                    TaxManager.PricesIncludeTax = cbPricesIncludeTax.Checked;
                    TaxManager.AllowCustomersToSelectTaxDisplayType = cbAllowCustomersToSelectTaxDisplayType.Checked;
                    TaxManager.TaxDisplayType = (TaxDisplayTypeEnum)Enum.ToObject(typeof(TaxDisplayTypeEnum), int.Parse(this.ddlTaxDisplayType.SelectedItem.Value));
                    TaxManager.DisplayTaxSuffix = cbDisplayTaxSuffix.Checked;
                    TaxManager.DisplayTaxRates = cbDisplayTaxRates.Checked;
                    TaxManager.HideZeroTax= cbHideZeroTax.Checked;
                    TaxManager.HideTaxInOrderSummary = cbHideTaxInOrderSummary.Checked; 
                    TaxManager.TaxBasedOn = (TaxBasedOnEnum)Enum.ToObject(typeof(TaxBasedOnEnum), int.Parse(this.ddlTaxBasedOn.SelectedItem.Value));

                    //tax address
                    Address defaultTaxAddress = new Address();
                    defaultTaxAddress.CountryId = int.Parse(this.ddlTaxDefaultCountry.SelectedItem.Value);
                    defaultTaxAddress.StateProvinceId = int.Parse(this.ddlTaxDefaultStateProvince.SelectedItem.Value);
                    defaultTaxAddress.ZipPostalCode = txtTaxDefaultZipPostalCode.Text;
                    TaxManager.DefaultTaxAddress = defaultTaxAddress;

                    //shipping
                    TaxManager.ShippingIsTaxable = cbShippingIsTaxable.Checked;
                    TaxManager.ShippingPriceIncludesTax = cbShippingPriceIncludesTax.Checked;
                    TaxManager.ShippingTaxClassId = int.Parse(this.ddlShippingTaxClass.SelectedItem.Value);

                    //additional payment fee
                    TaxManager.PaymentMethodAdditionalFeeIsTaxable = cbPaymentMethodAdditionalFeeIsTaxable.Checked;
                    TaxManager.PaymentMethodAdditionalFeeIncludesTax = cbPaymentMethodAdditionalFeeIncludesTax.Checked;
                    TaxManager.PaymentMethodAdditionalFeeTaxClassId = int.Parse(this.ddlPaymentMethodAdditionalFeeTaxClass.SelectedItem.Value);

                    //EU VAT support
                    TaxManager.EUVatEnabled = cbEUVatEnabled.Checked;
                    TaxManager.EUVatShopCountryId = int.Parse(this.ddlEUVatShopCountry.SelectedItem.Value);
                    TaxManager.EUVatAllowVATExemption = cbEUVatAllowVATExemption.Checked;
                    TaxManager.EUVatUseWebService = cbEUVatUseWebService.Checked;
                    TaxManager.EUVatEmailAdminWhenNewVATSubmitted = cbEUVatEmailAdminWhenNewVATSubmitted.Checked;

                    //logging
                    CustomerActivityManager.InsertActivity(
                        "EditTaxSettings",
                        GetLocaleResourceString("ActivityLog.EditTaxSettings"));

                    Response.Redirect("TaxSettings.aspx");
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

    }
}
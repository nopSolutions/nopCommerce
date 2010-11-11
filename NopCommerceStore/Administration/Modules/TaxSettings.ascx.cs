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
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

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

            var taxCategoryCollection = IoC.Resolve<ITaxCategoryService>().GetAllTaxCategories();

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
            var countries = IoC.Resolve<ICountryService>().GetAllCountries();

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

            var stateProvinceCollection = IoC.Resolve<IStateProvinceService>().GetStateProvincesByCountryId(countryId);
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
            var countries = IoC.Resolve<ICountryService>().GetAllCountries().Where(country => country.SubjectToVAT).ToList();

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
            cbPricesIncludeTax.Checked = IoC.Resolve<ITaxService>().PricesIncludeTax;
            cbAllowCustomersToSelectTaxDisplayType.Checked = IoC.Resolve<ITaxService>().AllowCustomersToSelectTaxDisplayType;
            CommonHelper.SelectListItem(this.ddlTaxDisplayType, (int)IoC.Resolve<ITaxService>().TaxDisplayType);
            cbDisplayTaxSuffix.Checked = IoC.Resolve<ITaxService>().DisplayTaxSuffix;
            cbDisplayTaxRates.Checked = IoC.Resolve<ITaxService>().DisplayTaxRates;
            cbHideZeroTax.Checked = IoC.Resolve<ITaxService>().HideZeroTax;
            cbHideTaxInOrderSummary.Checked = IoC.Resolve<ITaxService>().HideTaxInOrderSummary;
            CommonHelper.SelectListItem(this.ddlTaxBasedOn, (int)IoC.Resolve<ITaxService>().TaxBasedOn);

            //tax address
            Address defaultTaxAddress = IoC.Resolve<ITaxService>().DefaultTaxAddress;
            this.FillCountryDropDowns();
            CommonHelper.SelectListItem(this.ddlTaxDefaultCountry, defaultTaxAddress.CountryId);
            this.FillStateProvinceDropDowns();
            CommonHelper.SelectListItem(this.ddlTaxDefaultStateProvince, defaultTaxAddress.StateProvinceId);
            txtTaxDefaultZipPostalCode.Text = defaultTaxAddress.ZipPostalCode;

            //shipping
            cbShippingIsTaxable.Checked = IoC.Resolve<ITaxService>().ShippingIsTaxable;
            cbShippingPriceIncludesTax.Checked = IoC.Resolve<ITaxService>().ShippingPriceIncludesTax;
            CommonHelper.SelectListItem(this.ddlShippingTaxClass, IoC.Resolve<ITaxService>().ShippingTaxClassId);

            //additional payment fee
            cbPaymentMethodAdditionalFeeIsTaxable.Checked = IoC.Resolve<ITaxService>().PaymentMethodAdditionalFeeIsTaxable;
            cbPaymentMethodAdditionalFeeIncludesTax.Checked = IoC.Resolve<ITaxService>().PaymentMethodAdditionalFeeIncludesTax;
            CommonHelper.SelectListItem(this.ddlPaymentMethodAdditionalFeeTaxClass, IoC.Resolve<ITaxService>().PaymentMethodAdditionalFeeTaxClassId);

            //EU VAT support
            cbEUVatEnabled.Checked = IoC.Resolve<ITaxService>().EUVatEnabled;
            this.FillEUVATCountryDropDowns();
            CommonHelper.SelectListItem(this.ddlEUVatShopCountry, IoC.Resolve<ITaxService>().EUVatShopCountryId);
            cbEUVatAllowVATExemption.Checked = IoC.Resolve<ITaxService>().EUVatAllowVATExemption;
            cbEUVatUseWebService.Checked = IoC.Resolve<ITaxService>().EUVatUseWebService;
            cbEUVatEmailAdminWhenNewVATSubmitted.Checked = IoC.Resolve<ITaxService>().EUVatEmailAdminWhenNewVATSubmitted;
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
                    IoC.Resolve<ITaxService>().PricesIncludeTax = cbPricesIncludeTax.Checked;
                    IoC.Resolve<ITaxService>().AllowCustomersToSelectTaxDisplayType = cbAllowCustomersToSelectTaxDisplayType.Checked;
                    IoC.Resolve<ITaxService>().TaxDisplayType = (TaxDisplayTypeEnum)Enum.ToObject(typeof(TaxDisplayTypeEnum), int.Parse(this.ddlTaxDisplayType.SelectedItem.Value));
                    IoC.Resolve<ITaxService>().DisplayTaxSuffix = cbDisplayTaxSuffix.Checked;
                    IoC.Resolve<ITaxService>().DisplayTaxRates = cbDisplayTaxRates.Checked;
                    IoC.Resolve<ITaxService>().HideZeroTax= cbHideZeroTax.Checked;
                    IoC.Resolve<ITaxService>().HideTaxInOrderSummary = cbHideTaxInOrderSummary.Checked; 
                    IoC.Resolve<ITaxService>().TaxBasedOn = (TaxBasedOnEnum)Enum.ToObject(typeof(TaxBasedOnEnum), int.Parse(this.ddlTaxBasedOn.SelectedItem.Value));

                    //tax address
                    Address defaultTaxAddress = new Address();
                    defaultTaxAddress.CountryId = int.Parse(this.ddlTaxDefaultCountry.SelectedItem.Value);
                    defaultTaxAddress.StateProvinceId = int.Parse(this.ddlTaxDefaultStateProvince.SelectedItem.Value);
                    defaultTaxAddress.ZipPostalCode = txtTaxDefaultZipPostalCode.Text;
                    IoC.Resolve<ITaxService>().DefaultTaxAddress = defaultTaxAddress;

                    //shipping
                    IoC.Resolve<ITaxService>().ShippingIsTaxable = cbShippingIsTaxable.Checked;
                    IoC.Resolve<ITaxService>().ShippingPriceIncludesTax = cbShippingPriceIncludesTax.Checked;
                    IoC.Resolve<ITaxService>().ShippingTaxClassId = int.Parse(this.ddlShippingTaxClass.SelectedItem.Value);

                    //additional payment fee
                    IoC.Resolve<ITaxService>().PaymentMethodAdditionalFeeIsTaxable = cbPaymentMethodAdditionalFeeIsTaxable.Checked;
                    IoC.Resolve<ITaxService>().PaymentMethodAdditionalFeeIncludesTax = cbPaymentMethodAdditionalFeeIncludesTax.Checked;
                    IoC.Resolve<ITaxService>().PaymentMethodAdditionalFeeTaxClassId = int.Parse(this.ddlPaymentMethodAdditionalFeeTaxClass.SelectedItem.Value);

                    //EU VAT support
                    IoC.Resolve<ITaxService>().EUVatEnabled = cbEUVatEnabled.Checked;
                    IoC.Resolve<ITaxService>().EUVatShopCountryId = int.Parse(this.ddlEUVatShopCountry.SelectedItem.Value);
                    IoC.Resolve<ITaxService>().EUVatAllowVATExemption = cbEUVatAllowVATExemption.Checked;
                    IoC.Resolve<ITaxService>().EUVatUseWebService = cbEUVatUseWebService.Checked;
                    IoC.Resolve<ITaxService>().EUVatEmailAdminWhenNewVATSubmitted = cbEUVatEmailAdminWhenNewVATSubmitted.Checked;

                    //logging
                    IoC.Resolve<ICustomerActivityService>().InsertActivity(
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
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Admin.Models.Common;
using Nop.Core.Domain.Tax;
using Nop.Web.Framework;

namespace Nop.Admin.Models.Settings
{
    public class TaxSettingsModel
    {
        public TaxSettingsModel()
        {
            PaymentMethodAdditionalFeeTaxCategories = new List<SelectListItem>();
            ShippingTaxCategories = new List<SelectListItem>();
            EuVatShopCountries = new List<SelectListItem>();
            DefaultTaxAddress = new AddressModel();
        }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.PricesIncludeTax")]
        public bool PricesIncludeTax { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.AllowCustomersToSelectTaxDisplayType")]
        public bool AllowCustomersToSelectTaxDisplayType { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.TaxDisplayType")]
        public TaxDisplayType TaxDisplayType { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.DisplayTaxSuffix")]
        public bool DisplayTaxSuffix { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.DisplayTaxRates")]
        public bool DisplayTaxRates { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.HideZeroTax")]
        public bool HideZeroTax { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.HideTaxInOrderSummary")]
        public bool HideTaxInOrderSummary { get; set; }





        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.TaxBasedOn")]
        public TaxBasedOn TaxBasedOn { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.DefaultTaxAddress")]
        public AddressModel DefaultTaxAddress { get; set; }
       




        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.ShippingIsTaxable")]
        public bool ShippingIsTaxable { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.ShippingPriceIncludesTax")]
        public bool ShippingPriceIncludesTax { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.ShippingTaxClass")]
        public int ShippingTaxClassId { get; set; }





        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.PaymentMethodAdditionalFeeIsTaxable")]
        public bool PaymentMethodAdditionalFeeIsTaxable { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.PaymentMethodAdditionalFeeIncludesTax")]
        public bool PaymentMethodAdditionalFeeIncludesTax { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.PaymentMethodAdditionalFeeTaxClass")]
        public int PaymentMethodAdditionalFeeTaxClassId { get; set; }




        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.EuVatEnabled")]
        public bool EuVatEnabled { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.EuVatShopCountry")]
        public int EuVatShopCountryId { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.EuVatAllowVatExemption")]
        public bool EuVatAllowVatExemption { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.EuVatUseWebService")]
        public bool EuVatUseWebService { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.EuVatEmailAdminWhenNewVatSubmitted")]
        public bool EuVatEmailAdminWhenNewVatSubmitted { get; set; }




        public SelectList TaxDisplayTypeValues { get; set; }
        public SelectList TaxBasedOnValues { get; set; }
        public IList<SelectListItem> PaymentMethodAdditionalFeeTaxCategories { get; set; }
        public IList<SelectListItem> ShippingTaxCategories { get; set; }
        public IList<SelectListItem> EuVatShopCountries { get; set; }
    }
}